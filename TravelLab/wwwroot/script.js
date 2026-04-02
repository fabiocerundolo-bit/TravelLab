document.addEventListener('DOMContentLoaded', () => {
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

    // --- Caricamento statistiche dashboard ---
    async function loadStats() {
        try {
            const clientiRes = await fetch('/api/clienti');
            console.log('Clienti status:', clientiRes.status);
            if (clientiRes.ok) {
                const clienti = await clientiRes.json();
                console.log('Clienti count:', clienti.length);
                document.getElementById('stat-clienti').innerText = clienti.length;
            }

            const prenotazioniCountRes = await fetch('/api/prenotazioni/count');
            console.log('Prenotazioni status:', prenotazioniCountRes.status);
            if (prenotazioniCountRes.ok) {
                const data = await prenotazioniCountRes.json();
                console.log('Prenotazioni count:', data.count);
                document.getElementById('stat-prenotazioni').innerText = data.count;
            }

            const ricaviRes = await fetch('/api/statistiche/ricavi-mensili');
            console.log('Ricavi status:', ricaviRes.status);
            if (ricaviRes.ok) {
                const ricaviMensili = await ricaviRes.json();
                console.log('Ricavi mensili:', ricaviMensili);
                const totale = ricaviMensili.reduce((sum, item) => sum + item.ricavoTotale, 0);
                console.log('Totale ricavi:', totale);
                document.getElementById('stat-ricavi').innerText = totale.toFixed(2);
            }
        } catch (err) {
            console.error('Errore caricamento statistiche:', err);
        }
    }

    // --- Gestione modale form ---
    const modal = document.getElementById('form-container');
    const formClienteDiv = document.getElementById('form-cliente');
    const formViaggioDiv = document.getElementById('form-viaggio');
    const formPrenotazioneDiv = document.getElementById('form-prenotazione');
    const closeModal = document.getElementById('close-form');

    function showModal(activeForm) {
        formClienteDiv.style.display = 'none';
        formViaggioDiv.style.display = 'none';
        formPrenotazioneDiv.style.display = 'none';
        activeForm.style.display = 'block';
        modal.style.display = 'flex';
    }

    closeModal.addEventListener('click', () => {
        modal.style.display = 'none';
    });

    // --- Eventi sidebar ---
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

    document.getElementById('btn-nuovo-cliente').addEventListener('click', () => {
        showModal(formClienteDiv);
    });

    document.getElementById('btn-nuovo-viaggio').addEventListener('click', () => {
        showModal(formViaggioDiv);
    });

    document.getElementById('btn-nuova-prenotazione').addEventListener('click', async () => {
        // Popola i select prima di mostrare il modale
        await populateSelects();
        showModal(formPrenotazioneDiv);
    });

    async function populateSelects() {
        const clientiRes = await fetch('/api/clienti');
        const clienti = await clientiRes.json();
        const clienteSelect = document.getElementById('cliente_id');
        clienteSelect.innerHTML = '<option value="">Seleziona cliente</option>';
        clienti.forEach(c => {
            clienteSelect.innerHTML += `<option value="${c.id}">${c.nome} ${c.cognome}</option>`;
        });

        const viaggiRes = await fetch('/api/viaggi');
        const viaggi = await viaggiRes.json();
        const viaggioSelect = document.getElementById('viaggio_id');
        viaggioSelect.innerHTML = '<option value="">Seleziona viaggio</option>';
        viaggi.forEach(v => {
            viaggioSelect.innerHTML += `<option value="${v.id}">${v.destinazione} (${v.dataInizio} - ${v.dataFine})</option>`;
        });

        const agenzieRes = await fetch('/api/agenzie');
        const agenzie = await agenzieRes.json();
        const agenziaSelect = document.getElementById('agenzia_id');
        agenziaSelect.innerHTML = '<option value="">Seleziona agenzia</option>';
        agenzie.forEach(a => {
            agenziaSelect.innerHTML += `<option value="${a.id}">${a.nome}</option>`;
        });
    }

    // Submit forms (stessi listener di prima)
    document.getElementById('cliente-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {
            nome: document.getElementById('nome').value,
            cognome: document.getElementById('cognome').value,
            email: document.getElementById('email').value,
            telefono: document.getElementById('telefono').value,
            indirizzo: document.getElementById('indirizzo').value
        };
        try {
            const res = await fetch('/api/clienti', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (res.ok) {
                alert('Cliente aggiunto con successo!');
                modal.style.display = 'none';
                e.target.reset();
                loadStats(); // aggiorna contatore clienti
            } else {
                const error = await res.text();
                alert('Errore: ' + error);
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });

    document.getElementById('viaggio-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {
            descrizione: document.getElementById('descrizione').value,
            dataInizio: document.getElementById('data_inizio').value,
            dataFine: document.getElementById('data_fine').value,
            destinazione: document.getElementById('destinazione').value,
            prezzoBase: parseFloat(document.getElementById('prezzo_base').value)
        };
        try {
            const res = await fetch('/api/viaggi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (res.ok) {
                alert('Viaggio aggiunto con successo!');
                modal.style.display = 'none';
                e.target.reset();
            } else {
                const error = await res.text();
                alert('Errore: ' + error);
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });

    document.getElementById('prenotazione-form').addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {
            clienteId: parseInt(document.getElementById('cliente_id').value),
            viaggioId: parseInt(document.getElementById('viaggio_id').value),
            agenziaId: parseInt(document.getElementById('agenzia_id').value),
            dataPrenotazione: document.getElementById('data_prenotazione').value,
            stato: document.getElementById('stato').value
        };
        try {
            const res = await fetch('/api/prenotazioni', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (res.ok) {
                alert('Prenotazione aggiunta con successo!');
                modal.style.display = 'none';
                e.target.reset();
                loadStats(); // aggiorna contatore prenotazioni e ricavi
            } else {
                const error = await res.text();
                alert('Errore: ' + error);
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });

    // Carica statistiche iniziali
    loadStats();
});