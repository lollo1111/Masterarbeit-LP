const express = require('express');
const axios = require('axios');
const { spawn } = require('child_process');
let router = express.Router();
let sdk_health = 500;

router.get('/', async (req, res) => {
    const child = spawn('docker', ['ps']);
    let output = '';

    child.stdout.on('data', (data) => {
        output += data.toString();
    });

    child.stderr.on('data', (data) => {
        console.error(`stderr: ${data}`);
    });

    child.on('close', async (code) => {
        if (code !== 0) {
            return res.status(500).send(`Docker ps command failed with code ${code}`);
        }

        const containers = output.split('\n').slice(1, -1).map((line) => {
            const parts = line.split(/\s{2,}/);
            return {
                id: parts[0],
                image: parts[1],
                command: parts[2],
                created: parts[3],
                status: parts[4],
                ports: parts[5],
                names: parts[6],
            };
        });
        const mosquitto = containers.filter(container => container.names === "mosquitto")[0]["status"].startsWith("Up") ? 200 : 500;
        const kafka = containers.filter(container => container.names === "broker")[0]["status"].startsWith("Up") ? 200 : 500;
        const consumer = containers.filter(container => container.image === "consumer_image")[0]["status"].startsWith("Up") ? 200 : 500;
        const mqttBridge = containers.filter(container => container.image === "masterarbeit-lorenz-pircher-mqttbridge")[0]["status"].startsWith("Up") ? 200 : 500;

        const endpoints = [
            'http://host.docker.internal:7410/api/tags',
            'http://host.docker.internal:8081',
            'http://host.docker.internal:8298',
            'http://host.docker.internal:8080',
            'http://host.docker.internal:3000',
            'http://host.docker.internal:8086'
        ];
        try {
            const responses = await Promise.all(endpoints.map(endpoint => axios.get(endpoint).catch(error => ({ error }))));
            let statuses = responses.map(response => {
                if (response.error) {
                    console.error(`Error fetching ${response.error.config.url}: ${response.error.message}`)
                    return 500
                }
                return response.status
            })
            statuses.push(...[mosquitto, kafka, consumer, mqttBridge, sdk_health]);
            const openPLC = spawn('docker', ['logs', '--since', '30s', 'openplc']);
            let logs = '';
            openPLC.stdout.on('data', (data) => {
                logs += data.toString();
                const logMatch = logs.includes("Connection failed on MB device FactoryIO");
                if (logMatch) {
                    statuses[3] = 500;
                }
            });

            openPLC.stderr.on('data', (data) => {
                console.error(`stderr: ${data}`);
            });

            openPLC.on('close', async (_code) => {
                res.status(200).json(statuses);
            });
        } catch (err) {
            console.log(err);
        }
    });
});

router.get('/startContainer', (req, res) => {
    sdk_health = 200;
    res.status(200).send();
});

router.get('/endContainer', (req, res) => {
    sdk_health = 500;
    res.status(200).send();
});

module.exports = router;