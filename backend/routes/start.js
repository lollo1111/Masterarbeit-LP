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

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

async function readJson() {
    const data = await fs.readFile("./data/worklist.json", "utf8");
    const json = JSON.parse(Buffer.from(data));
    return json;
}

router.post('/', async (req, res) => {
    const id = crypto.randomUUID();
    let start_counter = await fetch('http://host.docker.internal:7410/api/tags/by-name/start_counter');
    let start_counter_amount = await start_counter.json();
    while (start_counter_amount[0]['value'] > 0) {
        console.log("Production Line is not free.");
        start_counter = await fetch('http://host.docker.internal:7410/api/tags/by-name/start_counter');
        start_counter_amount = await start_counter.json();
    }
    var callback = req.headers['cpee-callback']; // + "/";
    var taskName = req.headers['cpee-label'];
    const instance = req.headers["cpee-instance"];
    order = {
        //details
        id: id,
        instance: instance,
        callback: callback,
        currentTask: taskName,
        product: req.body.product,
        mirrorShape: req.body.mirrorShape !== "" ? req.body.mirrorShape : null,
        doorType: req.body.doorType !== "" ? req.body.doorType : null,
        tableStyle: req.body.tableStyle !== "" ? req.body.tableStyle : null,
        express: req.body.express,
        additionalEquipment: req.body.additionalEquipment,
        status: "In Progress"
    };
    Object.keys(order).forEach(function (key) {
        if (order[key] === null) {
            delete order[key];
        }
    });
    const body_on = [
        {
            name: "start_emitter",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    await timeout(100);
    const body_off = [
        {
            name: "start_emitter",
            value: false
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_off)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/finished', async (req, res) => {
    order.status = 'Completed';
    await fetch(order.callback, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            id: order.id
        })
    });
    res.status(200).send();
});

router.get('/init', async (req, res) => {
    var callback = req.headers['cpee-callback']; // + "/";
    var taskName = req.headers['cpee-label'];
    order.callback = callback;
    order.currentTask = taskName;
    order.status = "In Progress";
    const body_on = [
        {
            name: "simulate_init",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});


router.post('/machining', async (req, res) => {
    var callback = req.headers['cpee-callback']; // + "/";
    var taskName = req.headers['cpee-label'];
    let theOrder = map.get(req.body.reference);
    theOrder.status = "In Progress";
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    console.log("Machine: ", req.body.machine);
    theOrder.direction = req.body.machine === "A" ? 1 : 0;
    let body_on;
    if (req.body.machine === "A") {
        body_on = [
            {
                name: "machining_A_start",
                value: true
            }
        ];
    } else {
        body_on = [
            {
                name: "machining_B_start",
                value: true
            }
        ];
    }
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/task', (req, res) => {
    var callback = req.headers['cpee-callback']; // + "/";
    var taskName = req.headers['cpee-label'];
    let theOrder = map.get(req.body.reference);
    theOrder.status = "In Progress";
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    map.set(req.body.reference, theOrder);
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/details', (req, res) => {
    res.json(order);
});

router.post('/details', (req, res) => {
    res.json(map.get(req.body.reference));
});

router.post('/complete', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    map.set(req.body.reference, theOrder);
    await fetch(theOrder.callback, {
        method: 'PUT',
        // headers: {
        //     'Content-Type': 'application/json'
        // },
        // body: JSON.stringify({
        //     reference: req.body.reference
        // })
    });
    res.status(200).send();
});

router.post('/completeMirrorTasks', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.palletStatus = "Completed";
    map.set(req.body.reference, theOrder);
    await fetch(theOrder.palletCallback, {
        method: 'PUT',
    });
    console.log(JSON.stringify(theOrder));
    res.status(200).send();
});

// RFID Transponder Initialization
// wird von SDK geschickt und Daten werden in CPEE geladen
router.post('/setup', async (req, res) => {
    order.status = "Completed";
    map.set(req.body.reference, order);
    await fetch(order.callback, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            reference: req.body.reference
        })
    });
    return res.json(map.get(req.body.reference));
});

router.post('/completeQuality', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    theOrder.status = "Completed";
    map.set(req.body.reference, theOrder);
    await fetch(theOrder.callback, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            quality: req.body.quality
        })
    });
    return res.json(map.get(req.body.reference));
});

router.post('/setstatus', (req, res) => {
    let status;
    if (req.body.code === 0) {
        status = "Completed";
    } else if (req.body.code === 1) {
        status = "Leaving";
    } else if (req.body.code === 2) {
        status = "In Progress";
    }
    let theOrder = map.get(req.body.reference);
    theOrder.status = status;
    map.set(req.body.reference, theOrder);
    res.status(200).json(map.get(req.body.reference));
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

router.post('/pack', (req, res) => {
    let theOrder = map.get(req.body.reference);
    if (req.body.box === "S") {
        theOrder.box = 1;
    } else if (req.body.box === "M") {
        theOrder.box = 2;
    } else {
        // Paket L
        theOrder.box = 4;
    }
    map.set(req.body.reference, theOrder);
    console.log("Box Order: ", map.get(req.body.reference));
    res.send();
});

router.post('/logisticOption', (req, res) => {
    let theOrder = map.get(req.body.reference);
    if (req.body.logisticOption === "Express") {
        theOrder.logisticOption = 1;
    } else if (req.body.logisticOption === "Palette") {
        theOrder.logisticOption = 3;
    } else {
        // Standard
        theOrder.logisticOption = 2;
    }
    map.set(req.body.reference, theOrder);
    console.log("LogisticOption Order: ", map.get(req.body.reference));
    res.send();
});

router.post('/determineQuality', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    const quality = Math.random() < 0.5;
    // const quality = true;
    if (quality) {
        theOrder.qualityAcceptable = true;
    } else {
        // Ungenügend
        theOrder.qualityAcceptable = false;
    }
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_quality_check",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/classicTable', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "classic",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/modernTable', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "modern",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/tableLegs', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_table_legs_production",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
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

router.post('/preDrill', async (req, res) => {
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    let theOrder = map.get(req.body.reference);
    theOrder.status = "In Progress";
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: theOrder.product === "schreibtisch" ? "simulate_pre_drill" : "simulate_pre_drill_schrank",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/drawers', async (req, res) => {
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    let theOrder = map.get(req.body.reference);
    theOrder.status = "In Progress";
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_drawers_production",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/shelves', async (req, res) => {
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    let theOrder = map.get(req.body.reference);
    theOrder.status = "In Progress";
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_shelves_production",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/defaultDoor', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "defaultDoor",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/slidingDoor', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "slidingDoor",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/lock', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_lock_production",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/assemble', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_assemble",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/extras', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_extra_parts",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/improve', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_improve_quality",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/equipment', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.callback = callback;
    theOrder.currentTask = taskName;
    theOrder.status = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_additional_equipment",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/mirrorMaterial', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.palletCallback = callback;
    theOrder.currentPalletTask = taskName;
    theOrder.palletStatus = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "simulate_prepare_mirror_material",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/circularMirror', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.palletCallback = callback;
    theOrder.currentPalletTask = taskName;
    theOrder.palletStatus = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "circular",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.post('/angularMirror', async (req, res) => {
    let theOrder = map.get(req.body.reference);
    var callback = req.headers['cpee-callback'];
    var taskName = req.headers['cpee-label'];
    theOrder.palletCallback = callback;
    theOrder.currentPalletTask = taskName;
    theOrder.palletStatus = "In Progress";
    map.set(req.body.reference, theOrder);
    const body_on = [
        {
            name: "angular",
            value: true
        }
    ];
    await fetch('http://host.docker.internal:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_on)
    });
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/worklist', async (req, res) => {
    const json = await readJson();
    const response = await fetch("http://localhost:7410/api/tags");
    const devices = await response.json();
    const replaceDevicesWithObjects = (json, devices) => {
        return json.map(obj => {
            const devicesWithObjects = obj.devices.map(device => {
                const matchingDevice = devices.find(obj2 => obj2.name === device);
                return matchingDevice ? matchingDevice : device;
            });
            return { ...obj, devices: devicesWithObjects };
        });
    };
    const result = replaceDevicesWithObjects(json, devices);
    // const sensorObj = json.find(obj => obj.device === 'startsensor');
    // sensorObj.list.push("LOL");
    // console.log(JSON.stringify(json));
    // await fs.writeFile('./data/neu.json', JSON.stringify(json));
    res.status(200).json(result);
});

module.exports = router;