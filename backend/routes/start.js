const express = require('express');
const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const HashMap = require('hashmap');
const crypto = require('crypto');

let router = express.Router();
const map = new HashMap();
let order;

function timeout(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

router.post('/', async (req, res) => {
    const id = crypto.randomUUID();
    const start_sensor = await fetch('http://host.docker.internal:7410/api/tags/by-name/start_sensor');
    const start_sensor_status = await start_sensor.json();
    const start_rfid_sensor = await fetch('http://host.docker.internal:7410/api/tags/by-name/start_rfid_sensor');
    const start_rfid_sensor_status = await start_rfid_sensor.json();
    if (start_sensor_status[0]['value'] || start_rfid_sensor_status[0]['value']) return console.log("Production Line is not free.");
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
        additionalEquipment: req.body.additionalEquipment
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

router.get('/init', (req, res) => {
    var callback = req.headers['cpee-callback']; // + "/";
    var taskName = req.headers['cpee-label'];
    order.callback = callback;
    order.currentTask = taskName;
    res.status(200).setHeader('cpee-callback', true).send();
});

router.get('/details', (req, res) => {
    res.json(order);
});

// RFID Transponder Initialization
// wird von SDK geschickt und Daten werden in CPEE geladen
router.post('/setup', async (req, res) => {
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

module.exports = router;