const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const io = require('socket.io-client');
socket = io.connect('http://abgabe.cs.univie.ac.at:9033');

function timeout(ms) {
  return new Promise(resolve => setTimeout(resolve, ms));
}

socket.on('connect', function () {
  console.log("Socket connected");
});

socket.on('start', async (arg) => {
  console.log("Process is starting ...");
  const production_line = await fetch('http://localhost:7410/api/tags/by-name/X1_Antrieb');
  const production_line_free = await production_line.json();
  if (production_line_free[0]['value']) return console.log("Production Line is not free.");
  const body_on = [
    {
      name: "X_Emit",
      value: true
    }
  ];
  const emit_on = await fetch('http://localhost:7410/api/tag/values/by-name', {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(body_on)
  });
  if (emit_on.ok) {
    await timeout(100);
    const instance_body = [
      {
        "name": "RFID_Write_1",
        "value": +arg
      }
    ];
    const rfid_instance = await fetch('http://localhost:7410/api/tag/values/by-name', {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(instance_body)
    });
    if (rfid_instance.ok) {
      const body_off = [
        {
          name: "X_Emit",
          value: false
        }
      ];
      const emit_off = await fetch('http://localhost:7410/api/tag/values/by-name', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(body_off)
      });
      if (emit_off.ok) {
        return console.log("Product successfully emited.")
      }
    }
  }
  return console.log("Error during emit.");
});