const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const express = require('express');
const app = express();
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server);
const HashMap = require('hashmap');
const map = new HashMap();

app.get('/', async (req, res) => {
    const production_line = await fetch('http://host.docker.internal:7410/api/tags/by-name/direction_stopper');
    const production_line_free = await production_line.json();
    if (production_line_free[0]['value']) return console.log("Production Line is not free.");
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
    console.log("Gotcha!");
    res.status(200).json({
        lifecheck: "Success"
    });
});

app.post('/start', (req, res) => {
    console.log(req.headers);
    const instance = req.headers["cpee-instance"];
    map.set(instance, req.body);
    // console.log({
    //     key: instance,
    //     value: map.get(instance)
    // });
    try {
        io.emit("start", instance);
    } catch (_err) {
        return res.json({
            code: false
        });
    }
    return res.json({
        code: true
    });
});

// app.post('/setup', (req, res) => {
//     map.set(req.body.sid, map.get(req.body.instance));
//     map.delete(req.body.instance);
//     res.json(map.get(req.body.sid));
// });

app.post('/setup', (req, res) => {
    const instance = req.body.instance;
    res.json(map.get(instance));
});

io.on("connection", () => {
    console.log("New Connection");
});

const port = 9033;

server.listen(port, () => {
    console.log('listening on *:9033');
});