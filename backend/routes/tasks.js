const express = require('express');
const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const HashMap = require('hashmap');
const crypto = require('crypto');
const fs = require('fs').promises;
const lockfile = require('lockfile');

let router = express.Router();
const map = new HashMap();
let order;
let desks = [];
let wardrobes = [];
let mirrors = [];
let boxes = [];
let pallet = [];

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function readJson() {
    const data = await fs.readFile("./data/devices.json", "utf8");
    const json = JSON.parse(Buffer.from(data));
    return json;
}

async function writeJson(simulation, enter = true, palletTask = false) {
    let json = await readJson();
    const lockfilePath = './data/devices.json.lock';
    await new Promise((resolve, reject) => {
        lockfile.lock(lockfilePath, (err) => {
            if (err) {
                reject(err);
            } else {
                resolve();
            }
        });
    });
    if (simulation.currentTask === "Erneute Qualitätskontrolle") simulation.currentTask = "Qualitätskontrolle";
    let currentTask;
    if (palletTask) {
        currentTask = json.find(obj => obj.task === simulation.currentPalletTask);
    } else {
        currentTask = json.find(obj => obj.task === simulation.currentTask);
    }
    if (enter) {
        if (currentTask) {
            currentTask.items.push(simulation);
        } else {
            console.log("Nicht gefunden: " + simulation.currentTask);
        }
    } else {
        if (currentTask) {
            currentTask.items = currentTask.items.filter(item => item.id !== simulation.id);
        } else {
            console.log("Nicht gefunden: " + simulation.currentTask);
        }
    }
    await fs.writeFile('./data/devices.json', JSON.stringify(json));
    await new Promise((resolve, reject) => {
        lockfile.unlock(lockfilePath, (err) => {
            if (err) {
                reject(err);
            } else {
                resolve();
            }
        });
    });
}

async function updatePart(partName, value) {
    const body_on = [
        {
            name: partName,
            value: value
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
}

async function getPart(partName) {
    const response = await fetch('http://host.docker.internal:7410/api/tags/by-name/' + partName);
    const msg = await response.json();
    return msg;
}

async function sendCallback(callback, jsonBody = null) {
    if (!jsonBody) {
        await fetch(callback, {
            method: 'PUT',
        });
    } else {
        await fetch(callback, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(jsonBody)
        });
    }
}

async function registerTask(req, reference = null, palletTask = false) {
    const callback = req.headers['cpee-callback'].replace("https", "http").replace("localhost", "host.docker.internal");
    const taskName = req.headers['cpee-label'];
    if (!reference) {
        const instance = req.headers["cpee-instance"];
        return {
            callback: callback,
            taskName: taskName,
            instance: instance
        };
    }
    let theOrder = map.get(reference);
    if (!palletTask) {
        theOrder.status = "In Progress";
        theOrder.callback = callback;
        theOrder.currentTask = taskName;
        await writeJson(theOrder);
    } else {
        theOrder.palletCallback = callback;
        theOrder.currentPalletTask = taskName;
        theOrder.palletStatus = "In Progress";
        await writeJson(theOrder, true, true);
    }
    return theOrder;
}

router.post('/start', async (req, res) => {
    const id = crypto.randomUUID();
    let start_counter_amount = await getPart("start_counter");
    while (start_counter_amount[0]['value'] > 0) {
        console.log("Production Line is not free.");
        start_counter_amount = await getPart("start_counter");
    }
    const reqisteredTask = await registerTask(req);
    order = {
        id: id,
        instance: reqisteredTask.instance,
        callback: reqisteredTask.callback,
        currentTask: reqisteredTask.taskName,
        product: req.body.product,
        mirrorShape: req.body.mirrorShape !== "" ? req.body.mirrorShape : null,
        doorType: req.body.doorType !== "" ? req.body.doorType : null,
        tableStyle: req.body.tableStyle !== "" ? req.body.tableStyle : null,
        express: req.body.express,
        additionalEquipment: req.body.additionalEquipment,
        status: "In Progress"
    };
    await writeJson(order);
    Object.keys(order).forEach(function (key) {
        if (order[key] === null) {
            delete order[key];
        }
    });
    await updatePart("start_emitter_part", order.product === "schrank" ? 32 : 16);
    await updatePart("start_emitter", true);
    await timeout(100);
    await updatePart("start_emitter", false);
    res.status(201).setHeader('cpee-callback', true).send();
});

router.patch('/placed', async (req, res) => {
    order.status = 'Completed';
    await writeJson(order, false);
    await sendCallback(order.callback, { id: order.id });
    res.status(200).send();
});

router.get('/init', async (req, res) => {
    const reqisteredTask = await registerTask(req);
    order.callback = reqisteredTask.callback;
    order.currentTask = reqisteredTask.taskName;
    order.status = "In Progress";
    await writeJson(order, true);
    await updatePart("simulate_init", true);
    res.status(200).setHeader('cpee-callback', true).send();
});


router.put('/machining', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    theOrder.direction = req.body.machine === "A" ? 1 : 0;
    if (req.body.machine === "A") {
        await updatePart("machining_A_start", true);
    } else {
        await updatePart("machining_B_start", true);
    }
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/varnishing', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/details', (req, res) => {
    res.json(map.get(req.body.reference));
});

router.put('/height', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.patch('/determineHeight', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    theOrder.height = req.body.height;
    await writeJson(theOrder, false);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback, { height: req.body.height });
    return res.json(map.get(req.body.reference));
});

router.put('/weight', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.patch('/determineWeight', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    theOrder.weight = req.body.weight;
    await writeJson(theOrder, false);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback, { weight: req.body.weight });
    return res.json(map.get(req.body.reference));
});

router.patch('/complete', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    await writeJson(theOrder, false);
    if (theOrder.currentTask !== "Produkt versenden") {
        map.set(req.body.reference, theOrder);
    } else {
        map.delete(req.body.reference);
    }
    await sendCallback(theOrder.callback);
    res.status(200).send();
});

router.patch('/completeMirrorTasks', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.palletStatus = "Completed";
    await writeJson(theOrder, false, true);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.palletCallback);
    res.status(200).send();
});

router.patch('/setup', async (req, res) => {
    order.status = "Completed";
    await writeJson(order, false);
    map.set(req.body.reference, order);
    await sendCallback(order.callback, { reference: req.body.reference });
    return res.json(map.get(req.body.reference));
});

router.patch('/determineQuality', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    await writeJson(theOrder, false);
    theOrder.status = "Completed";
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback, { quality: req.body.quality });
    return res.json(map.get(req.body.reference));
});

router.post('/saveWardrobe', (req, res) => {
    wardrobes.push(req.body.reference);
    res.send();
});

router.post('/saveDesk', (req, res) => {
    desks.push(req.body.reference);
    res.send();
});

router.post('/mirrorReference', (req, res) => {
    mirrors.push(req.body.reference);
    res.send();
});

router.post('/boxReference', (req, res) => {
    boxes.push(req.body.reference);
    res.send();
});

router.put('/pack', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    if (req.body.box === "S") {
        theOrder.box = 1;
    } else if (req.body.box === "M") {
        theOrder.box = 2;
    } else {
        // Paket L
        theOrder.box = 4;
    }
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/logisticOption', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    if (req.body.logisticOption === "Express") {
        theOrder.logisticOption = 1;
    } else if (req.body.logisticOption === "Palette") {
        theOrder.logisticOption = 3;
    } else {
        // Standard
        theOrder.logisticOption = 2;
    }
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/quality', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    const quality = Math.random() < 0.8;
    if (quality) {
        theOrder.qualityAcceptable = true;
    } else {
        // Ungenügend
        theOrder.qualityAcceptable = false;
    }
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_quality_check", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/classicTable', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("classic", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/modernTable', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("modern", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/tableLegs', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_table_legs_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/prepareWardrobe', (req, res) => {
    const ref = wardrobes.shift();
    res.status(200).json({
        reference: ref
    })
});

router.get('/prepareDesk', (req, res) => {
    const ref = desks.shift();
    res.status(200).json({
        reference: ref
    })
});

router.get('/prepareMirror', (req, res) => {
    const ref = mirrors.shift();
    res.status(200).json({
        reference: ref
    })
});

router.get('/prepareBox', (req, res) => {
    const ref = boxes.shift();
    res.status(200).json({
        reference: ref
    })
});

router.put('/addToPallet', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    if (pallet.length === 0) {
        pallet.push(req.body.reference);
    } else if (pallet.length === 1) {
        pallet.push(req.body.reference);
        await updatePart("pre_palletizer_stopper", true);
    } else if (pallet.length === 2) {
        pallet = [req.body.reference];
    }
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/palletContent', (req, res) => {
    res.json(pallet);
});

router.put('/preDrill', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart(theOrder.product === "schreibtisch" ? "simulate_pre_drill" : "simulate_pre_drill_wardrobe", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/drawers', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_drawers_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/shelves', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_shelves_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/defaultDoor', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("defaultDoor", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/slidingDoor', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("slidingDoor", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/lock', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_lock_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/assemble', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_assemble", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/extras', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_extra_parts", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/improve', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_improve_quality", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/equipment', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_additional_equipment", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/mirrorMaterial', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference, true);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_prepare_mirror_material", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/circularMirror', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference, true);
    map.set(req.body.reference, theOrder);
    await updatePart("circular", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/angularMirror', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("angular", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.put('/send', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

module.exports = router;