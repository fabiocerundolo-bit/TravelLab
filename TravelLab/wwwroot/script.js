// Funzione generica per fetch e visualizzazione
async function fetchData(url, buildTable) {
    try {
        const response = await fetch(url);
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        const data = await response.json();
        const contentDiv = document.getElementById('content');

        if (!data || data.length === 0) {
            contentDiv.innerHTML = '<p>Nessun dato trovato.</p>';
            return;
        }

        const table = buildTable(data);
        contentDiv.innerHTML = '';
        contentDiv.appendChild(table);
    } catch (error) {
        console.error('Errore:', error);
        document.getElementById('content').innerHTML = `<p style="color:red;">Errore di connessione: ${error.message}</p>`;
    }
}

// Costruisce una tabella HTML a partire da un array di oggetti
function buildTableFromData(data) {
    const table = document.createElement('table');
    const thead = document.createElement('thead');
    const headerRow = document.createElement('tr');
    const columns = Object.keys(data[0]);

    columns.forEach(col => {
        const th = document.createElement('th');
        th.textContent = col;
        headerRow.appendChild(th);
    });
    thead.appendChild(headerRow);
    table.appendChild(thead);

    const tbody = document.createElement('tbody');
    data.forEach(row => {
        const tr = document.createElement('tr');
        columns.forEach(col => {
            const td = document.createElement('td');
            td.textContent = row[col] !== null && row[col] !== undefined ? row[col] : '';
            tr.appendChild(td);
        });
        tbody.appendChild(tr);
    });
    table.appendChild(tbody);
    return table;
}

// Eventi pulsanti
document.getElementById('btn-clienti').addEventListener('click', () => {
    fetchData('/api/clienti', buildTableFromData);
});

document.getElementById('btn-prenotazioni-cliente').addEventListener('click', () => {
    const clienteId = prompt('Inserisci ID cliente:');
    if (!clienteId) return;
    fetchData(`/api/prenotazioni/cliente/${clienteId}`, buildTableFromData);
});

document.getElementById('btn-voli').addEventListener('click', () => {
    const destinazione = prompt('Destinazione (lasciare vuoto per tutte):');
    const dataInizio = prompt('Data inizio (YYYY-MM-DD, opzionale):');
    const dataFine = prompt('Data fine (YYYY-MM-DD, opzionale):');
    let url = '/api/voli?';
    const params = [];
    if (destinazione) params.push(`destinazione=${encodeURIComponent(destinazione)}`);
    if (dataInizio) params.push(`data_inizio=${dataInizio}`);
    if (dataFine) params.push(`data_fine=${dataFine}`);
    url += params.join('&');
    fetchData(url, buildTableFromData);
});

document.getElementById('btn-top-dest').addEventListener('click', () => {
    fetchData('/api/statistiche/top-destinazioni', buildTableFromData);
});

document.getElementById('btn-ricavi').addEventListener('click', () => {
    fetchData('/api/statistiche/ricavi-mensili', buildTableFromData);
});

document.getElementById('btn-clienti-senza').addEventListener('click', () => {
    fetchData('/api/clienti/senza-prenotazioni', buildTableFromData);
});