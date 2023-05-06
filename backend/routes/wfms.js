const express = require('express');
const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const fs = require('fs').promises;
let router = express.Router();

async function readFile(filename) {
    const data = await fs.readFile(("./data/xmls/" + filename), "utf8");
    return data;
}

async function readJson() {
    const data = await fs.readFile("./data/worklist.json", "utf8");
    const json = JSON.parse(Buffer.from(data));
    return json;
}

router.get('/worklist', async (req, res) => {
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
    // const sensorObj = json.find(obj => obj.device === 'startsensor');
    // sensorObj.list.push("LOL");
    // console.log(JSON.stringify(json));
    // await fs.writeFile('./data/neu.json', JSON.stringify(json));
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