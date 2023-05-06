const express = require('express');
const bodyParser = require('body-parser');
var cors = require('cors')

const app = express();
app.use(cors())
app.use(bodyParser.urlencoded({ extended: false }));
app.use(express.json());
const port = 9033;

// Import Routes
const taskMethods = require('./routes/tasks');
const healthcheckMethods = require('./routes/healthcheck');
const wfmsMethods = require('./routes/wfms');

// Use Routes
app.use('/tasks', taskMethods);
app.use('/healthcheck', healthcheckMethods);
app.use('/wfms', wfmsMethods);

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