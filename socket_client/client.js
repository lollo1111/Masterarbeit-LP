const fetch = (...args) => import('node-fetch').then(({ default: fetch }) => fetch(...args));
const io = require('socket.io-client');
socket = io.connect('http://abgabe.cs.univie.ac.at:9033');

socket.on('connect', function () {
  console.log("Socket connected");
});

socket.on('start', async function () {
  console.log("Process is starting ...");
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
  return console.log("Error during emit.");
});