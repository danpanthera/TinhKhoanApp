const statusEl = document.getElementById('status');

async function checkReady() {
  try {
    const res = await fetch('http://localhost:5055/ready');
    if (!res.ok) throw new Error(res.statusText);
    const data = await res.json();
    statusEl.textContent = data.ready ? 'ready' : 'not ready';
    statusEl.style.color = data.ready ? 'green' : 'orange';
  } catch (e) {
    statusEl.textContent = 'error: ' + e.message;
    statusEl.style.color = 'red';
  }
}

checkReady();
setInterval(checkReady, 5000);
