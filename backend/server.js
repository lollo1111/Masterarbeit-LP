const express = require('express');
const bodyParser = require('body-parser');
const app = express();
app.use(bodyParser.urlencoded({ extended: false }));
app.use(express.json());
const port = 9033;

// Import Routes
const startMethod = require('./routes/start');

// Use Routes
app.use('/start', startMethod);

app.listen(port, () => {
    console.log('listening on *:9033');
});







// // old socket io code
// const { Server } = require("socket.io");
// const io = new Server(server);
// io.on("connection", () => {
//     console.log("New Connection");
// });
// try {
//     io.emit("start", instance);
// } catch (_err) {
//     return res.json({
//         code: false
//     });
// }
// return res.json({
//     code: true
// });