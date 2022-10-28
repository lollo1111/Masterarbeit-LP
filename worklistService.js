const express = require('express');
const router = express.Router();
const fs = require('fs');

router.post('/', (req, res) => {
    var role = req.body.role;
    var linkToHtml =  req.body.linkToHtml;
    var callback = req.headers['cpee-callback']; // + "/";
    var taskName = req.headers['cpee-label'];
    var contextData = req.body.contextData;
    var randString = Date.now();
    var dokName = randString + ".txt";
    var data = {"role": role, "linkToHtml": linkToHtml, "taskName": taskName, "contextData": contextData, "user": "open", "cpee": callback, "dokName": dokName};
    var jsonData = JSON.stringify(data);
    fs.writeFile("tasks/" + randString + ".txt", jsonData, function(err) {
        if (err) {
            console.log(err);
        }
    });
    res.setHeader('cpee-callback', true);
    res.type('json');
    res.json(numb);
});

module.exports = router;
