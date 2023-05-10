const express = require('express');
const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const HashMap = require('hashmap');
const crypto = require('crypto');
const fs = require('fs').promises;

let router = express.Router();
const map = new HashMap();
let order;
let schreibtische = [];
let schraenke = [];
let mirrors = [];
let boxes = [];
let palette = [];

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function readJson() {
    const data = await fs.readFile("./data/worklist.json", "utf8");
    const json = JSON.parse(Buffer.from(data));
    return json;
}

async function writeJson(simulation, enter = true, palletTask = false) {
    let json = await readJson();
    let currentTask
    if (palletTask) {
        currentTask = json.find(obj => obj.task === simulation.currentPalletTask);
    } else {
        currentTask = json.find(obj => obj.task === simulation.currentTask);
    }
    if (enter) {
        if (currentTask) {
            currentTask.items.push(simulation);
        }
    } else {
        if (currentTask) {
            currentTask.items = currentTask.items.filter(item => item.id !== simulation.id);
        }
    }
    // await fs.writeFile('./data/neu.json', JSON.stringify(json));
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
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/finished', async (req, res) => {
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


router.post('/machining', async (req, res) => {
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

router.post('/task', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/details', (req, res) => {
    res.json(map.get(req.body.reference));
});

router.post('/height', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/determineHeight', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    theOrder.height = req.body.height;
    await writeJson(theOrder, false);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback, { height: req.body.height });
    return res.json(map.get(req.body.reference));
});

router.post('/weight', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/determineWeight', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    theOrder.weight = req.body.weight;
    await writeJson(theOrder, false);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback, { weight: req.body.weight });
    return res.json(map.get(req.body.reference));
});

router.post('/complete', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    await writeJson(theOrder, false);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback);
    res.status(200).send();
});

router.post('/completeMirrorTasks', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.palletStatus = "Completed";
    await writeJson(theOrder, false, true);
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.palletCallback);
    res.status(200).send();
});

// RFID Transponder Initialization
// wird von SDK geschickt und Daten werden in CPEE geladen
router.post('/setup', async (req, res) => {
    order.status = "Completed";
    await writeJson(order, false);
    map.set(req.body.reference, order);
    await sendCallback(order.callback, { reference: req.body.reference });
    return res.json(map.get(req.body.reference));
});

router.post('/completeQuality', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    await writeJson(theOrder, false);
    theOrder.status = "Completed";
    map.set(req.body.reference, theOrder);
    await sendCallback(theOrder.callback, { quality: req.body.quality });
    return res.json(map.get(req.body.reference));
});

router.post('/saveSchrank', (req, res) => {
    schraenke.push(req.body.reference);
    res.send();
});

router.post('/saveSchreibtisch', (req, res) => {
    schreibtische.push(req.body.reference);
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

router.post('/pack', async (req, res) => {
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

router.post('/logisticOption', async (req, res) => {
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

router.post('/determineQuality', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    const quality = Math.random() < 0.8;
    // const quality = true;
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

router.post('/classicTable', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("classic", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/modernTable', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("modern", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/tableLegs', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_table_legs_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/prepareSchrank', (req, res) => {
    const ref = schraenke.shift();
    res.status(200).json({
        reference: ref
    })
});

router.get('/prepareSchreibtisch', (req, res) => {
    const ref = schreibtische.shift();
    res.status(200).json({
        reference: ref
    })
});

router.get('/prepareSpiegel', (req, res) => {
    const ref = mirrors.shift();
    res.status(200).json({
        reference: ref
    })
});

router.get('/preparePaket', (req, res) => {
    const ref = boxes.shift();
    res.status(200).json({
        reference: ref
    })
});

router.post('/addToPallet', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    if (palette.length === 0) {
        palette.push(req.body.reference);
    } else if (palette.length === 1) {
        palette.push(req.body.reference);
        await updatePart("pre_palletizer_stopper", true);
    } else if (palette.length === 2) {
        palette = [req.body.reference];
    }
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/palletContent', (req, res) => {
    res.json(palette);
});

router.post('/preDrill', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart(theOrder.product === "schreibtisch" ? "simulate_pre_drill" : "simulate_pre_drill_schrank", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/drawers', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_drawers_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/shelves', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_shelves_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/defaultDoor', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("defaultDoor", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/slidingDoor', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("slidingDoor", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/lock', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_lock_production", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/assemble', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_assemble", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/extras', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_extra_parts", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/improve', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_improve_quality", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/equipment', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_additional_equipment", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/mirrorMaterial', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference, true);
    map.set(req.body.reference, theOrder);
    await updatePart("simulate_prepare_mirror_material", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/circularMirror', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference, true);
    map.set(req.body.reference, theOrder);
    await updatePart("circular", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/angularMirror', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    await updatePart("angular", true);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/send', async (req, res) => {
    let theOrder = await registerTask(req, req.body.reference);
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

module.exports = router;