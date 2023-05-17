const express = require('express');
const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const fs = require('fs').promises;
let router = express.Router();

async function readFile(filename, path="./data/xmls/") {
    const data = await fs.readFile((path + filename), "utf8");
    return data;
}

async function readJson() {
    const data = await fs.readFile("./data/devices.json", "utf8");
    const json = JSON.parse(Buffer.from(data));
    return json;
}

router.get('/devices', async (req, res) => {
    const json = await readJson();
    const response = await fetch("http://host.docker.internal:7410/api/tags");
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
    res.status(200).json(result);
});

router.get('/files', async (req, res) => {
    try {
        const files = await fs.readdir('./data/xmls');
        res.status(200).json(files);
    } catch (error) {
        console.error(error);
    }
});

router.post('/createFile', async (req, res) => {
    let xmlString = await readFile("template.xml", "./data/")
    xmlString = xmlString.replace("{{product}}", req.body.product).replace("{{express}}", req.body.express).replace("{{additionalEquipment}}", req.body.additionalEquipment).replace("{{maxHeight}}", req.body.maxHeight).replace("{{maxWeight}}", req.body.maxWeight).replace("{{Big}}", req.body.conditionBig).replace("{{Small}}", req.body.conditionSmall).replace("{{BoxS}}", req.body.conditionS).replace("{{BoxM}}", req.body.conditionM);
    if (req.body.product === "schrank") {
        xmlString = xmlString.replace("{{doorType}}", req.body.doorType).replace("{{mirrorShape}}", req.body.mirrorShape).replace("{{tableStyle}}", "");
    } else {
        xmlString = xmlString.replace("{{tableStyle}}", req.body.tableStyle).replace("{{doorType}}", "").replace("{{mirrorShape}}", "");
    }
    await fs.writeFile(('./data/xmls/' + req.body.name + '.xml'), xmlString);
    res.status(201).send();
});

router.delete('/deleteFile/:fileId', async (req, res) => {
    const fileId = req.params.fileId;
    try {
        await fs.unlink('./data/xmls/' + fileId + '.xml');
        res.status(204).send();
    } catch (err) {
        console.error(err);
    }
});

router.get('/download/:fileId', async (req, res) => {
    const fileId = req.params.fileId;
    const xml = await readFile((fileId + ".xml"));
    const filename = fileId + '.xml';
    res.set({
        'Content-Type': 'application/xml',
        'Content-Disposition': `attachment; filename="${filename}"`
    });
    res.status(200).send(xml);
});

router.get('/selectFile/:fileId', async (req, res) => {
    const fileId = req.params.fileId;
    const xml = await readFile((fileId + ".xml"));
    res.set('Content-Type', 'text/xml');
    res.status(200).send(xml);
});

module.exports = router;