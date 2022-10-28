const express = require('express');
const app = express();
app.use(express.json());
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server);

app.get('/', (req, res) => {
    res.json({
        lifecheck: "Success"
    });
});

app.get('/start', (req, res) => {
    try {
        io.emit("start");
    } catch(_err) {
        return res.json({
	    code: false		
        });
    }
    return res.json({
        code: true
    });
});

io.on("connection", () => {
    console.log("New Connection");
});

const port = 9033;

server.listen(port, () => {
    console.log('listening on *:9033');
});