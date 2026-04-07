// script.js - Dashboard amministrativa TravelLab
// Include autenticazione, fetch con credenziali, formattazione date

document.addEventListener('DOMContentLoaded', () => {
    // ------------------------------
    // Configurazione fetch per includere i cookie di autenticazione
    // ------------------------------
    const originalFetch = window.fetch;
    window.fetch = function(url, options) {
        options = options || {};
        options.credentials = 'include';
        return originalFetch(url, options);
    };

    // ------------------------------
    // Funzione generica per fetch e visualizzazione tabellare
    // ------------------------------
    async function fetchData(url, buildTable) {
        let response;
        try {
            response = await fetch(url);
            if (response.status === 401) {
                // Non autenticato: reindirizza al login
                window.location.href = '/login.html';
                return;
            }
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

    // ------------------------------
    // Costruisce una tabella HTML da un array di oggetti, formattando le date
    // ------------------------------
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
                let value = row[col] !== null && row[col] !== undefined ? row[col] : '';
                // Formatta le date ISO (YYYY-MM-DDTHH:MM:SS) in solo YYYY-MM-DD
                if (typeof value === 'string' && value.match(/^\d{4}-\d{2}-\d{2}T/)) {
                    value = value.split('T')[0];
                }
                td.textContent = value;
                tr.appendChild(td);
            });
            tbody.appendChild(tr);
        });
        table.appendChild(tbody);
        return table;
    }

    // ------------------------------
    // Caricamento statistiche dashboard (card)
    // ------------------------------
    async function loadStats() {
        let response;
        try {
            response = await fetch('/api/clienti');
            if (response.status === 401) {
                window.location.href = '/login.html';
                return;
            }
            if (response.ok) {
                const clienti = await response.json();
                document.getElementById('stat-clienti').innerText = clienti.length;
            } else {
                document.getElementById('stat-clienti').innerText = '?';
            }

            response = await fetch('/api/prenotazioni/count');
            if (response.ok) {
                const data = await response.json();
                document.getElementById('stat-prenotazioni').innerText = data.count;
            } else {
                document.getElementById('stat-prenotazioni').innerText = '?';
            }

            response = await fetch('/api/statistiche/ricavi-mensili');
            if (response.ok) {
                const ricaviMensili = await response.json();
                const totale = ricaviMensili.reduce((sum, item) => sum + (item.ricavoTotale || 0), 0);
                document.getElementById('stat-ricavi').innerText = totale.toFixed(2);
            } else {
                document.getElementById('stat-ricavi').innerText = '?';
            }
        } catch (err) {
            console.error('Errore caricamento statistiche:', err);
            document.getElementById('stat-clienti').innerText = '!';
            document.getElementById('stat-prenotazioni').innerText = '!';
            document.getElementById('stat-ricavi').innerText = '!';
        }
    }

    // ------------------------------
    // Gestione modale per i form di inserimento
    // ------------------------------
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

    if (closeModal) {
        closeModal.addEventListener('click', () => {
            modal.style.display = 'none';
        });
    }

    window.addEventListener('click', (e) => {
        if (e.target === modal) modal.style.display = 'none';
    });

    // ------------------------------
    // Eventi sidebar (pulsanti di visualizzazione)
    // ------------------------------
    document.getElementById('btn-clienti')?.addEventListener('click', () => {
        fetchData('/api/clienti', buildTableFromData);
    });

    document.getElementById('btn-prenotazioni-cliente')?.addEventListener('click', () => {
        const clienteId = prompt('Inserisci ID cliente:');
        if (!clienteId) return;
        fetchData(`/api/prenotazioni/cliente/${clienteId}`, buildTableFromData);
    });

    document.getElementById('btn-voli')?.addEventListener('click', () => {
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

    document.getElementById('btn-top-dest')?.addEventListener('click', () => {
        fetchData('/api/statistiche/top-destinazioni', buildTableFromData);
    });

    document.getElementById('btn-ricavi')?.addEventListener('click', () => {
        fetchData('/api/statistiche/ricavi-mensili', buildTableFromData);
    });

    document.getElementById('btn-clienti-senza')?.addEventListener('click', () => {
        fetchData('/api/clienti/senza-prenotazioni', buildTableFromData);
    });

    document.getElementById('btn-treni')?.addEventListener('click', () => {
        fetchData('/api/treni', buildTableFromData);
    });
    document.getElementById('btn-biglietti')?.addEventListener('click', () => {
        loadTickets();
    });

    document.getElementById('btn-navi')?.addEventListener('click', () => {
        fetchData('/api/navi', buildTableFromData);
    });

    // ------------------------------
    // Pulsanti per aprire i form di inserimento (modale)
    // ------------------------------
    document.getElementById('btn-nuovo-cliente')?.addEventListener('click', () => {
        showModal(formClienteDiv);
    });

    document.getElementById('btn-nuovo-viaggio')?.addEventListener('click', () => {
        showModal(formViaggioDiv);
    });

    document.getElementById('btn-nuova-prenotazione')?.addEventListener('click', async () => {
        await populateSelects();
        showModal(formPrenotazioneDiv);
    });

    // ------------------------------
    // Popola i select del form prenotazione (clienti, viaggi, agenzie)
    // ------------------------------
    async function populateSelects() {
        let response;
        try {
            response = await fetch('/api/clienti');
            if (response.ok) {
                const clienti = await response.json();
                const clienteSelect = document.getElementById('cliente_id');
                clienteSelect.innerHTML = '<option value="">Seleziona cliente</option>';
                clienti.forEach(c => {
                    clienteSelect.innerHTML += `<option value="${c.id}">${c.nome} ${c.cognome}</option>`;
                });
            }

            response = await fetch('/api/viaggi');
            if (response.ok) {
                const viaggi = await response.json();
                const viaggioSelect = document.getElementById('viaggio_id');
                viaggioSelect.innerHTML = '<option value="">Seleziona viaggio</option>';
                viaggi.forEach(v => {
                    const start = v.dataInizio?.split('T')[0] || v.dataInizio;
                    const end = v.dataFine?.split('T')[0] || v.dataFine;
                    viaggioSelect.innerHTML += `<option value="${v.id}">${v.destinazione} (${start} - ${end})</option>`;
                });
            }

            response = await fetch('/api/agenzie');
            if (response.ok) {
                const agenzie = await response.json();
                const agenziaSelect = document.getElementById('agenzia_id');
                agenziaSelect.innerHTML = '<option value="">Seleziona agenzia</option>';
                agenzie.forEach(a => {
                    agenziaSelect.innerHTML += `<option value="${a.id}">${a.nome}</option>`;
                });
            }
        } catch (err) {
            console.error('Errore nel popolamento dei select:', err);
        }
    }

    // ------------------------------
    // Submit forms (creazione clienti, viaggi, prenotazioni)
    // ------------------------------
    document.getElementById('cliente-form')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {
            nome: document.getElementById('nome').value,
            cognome: document.getElementById('cognome').value,
            email: document.getElementById('email').value,
            telefono: document.getElementById('telefono').value,
            indirizzo: document.getElementById('indirizzo').value
        };
        let response;
        try {
            response = await fetch('/api/clienti', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                alert('Cliente aggiunto con successo!');
                modal.style.display = 'none';
                e.target.reset();
                loadStats();
            } else {
                const error = await response.text();
                alert('Errore: ' + error);
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });

    document.getElementById('viaggio-form')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {
            descrizione: document.getElementById('descrizione').value,
            dataInizio: document.getElementById('data_inizio').value,
            dataFine: document.getElementById('data_fine').value,
            destinazione: document.getElementById('destinazione').value,
            prezzoBase: parseFloat(document.getElementById('prezzo_base').value)
        };
        let response;
        try {
            response = await fetch('/api/viaggi', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                alert('Viaggio aggiunto con successo!');
                modal.style.display = 'none';
                e.target.reset();
            } else {
                const error = await response.text();
                alert('Errore: ' + error);
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });

    document.getElementById('prenotazione-form')?.addEventListener('submit', async (e) => {
        e.preventDefault();
        const data = {
            clienteId: parseInt(document.getElementById('cliente_id').value),
            viaggioId: parseInt(document.getElementById('viaggio_id').value),
            agenziaId: parseInt(document.getElementById('agenzia_id').value),
            dataPrenotazione: document.getElementById('data_prenotazione').value,
            stato: document.getElementById('stato').value
        };
        let response;
        try {
            response = await fetch('/api/prenotazioni', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            if (response.ok) {
                alert('Prenotazione aggiunta con successo!');
                modal.style.display = 'none';
                e.target.reset();
                loadStats();
            } else {
                const error = await response.text();
                alert('Errore: ' + error);
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });

    // ------------------------------
    // Pulsante Logout
    // ------------------------------
    document.getElementById('btn-logout')?.addEventListener('click', async () => {
        let response;
        try {
            response = await fetch('/api/account/logout', { method: 'POST' });
            if (response.ok) {
                window.location.href = '/login.html';
            } else {
                alert('Errore durante il logout');
            }
        } catch (err) {
            alert('Errore di connessione');
        }
    });
    // ------------------------------
// Caricamento biglietti (esempio con endpoint /api/biglietti)
// ------------------------------
    // ------------------------------
// Caricamento biglietti (prenotazioni con dettagli viaggio)
// ------------------------------
    async function loadTickets() {
        const contentDiv = document.getElementById('content');
        if (!contentDiv) return;

        contentDiv.innerHTML = '<p>Caricamento biglietti...</p>';
        let response;
        try {
            response = await fetch('/api/prenotazioni'); // o l'endpoint corretto
            if (response.status === 401) {
                window.location.href = '/login.html';
                return;
            }
            if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);

            const data = await response.json();
            console.log('Dati ricevuti:', data); // per debug: controlla la struttura

            if (!data || data.length === 0) {
                contentDiv.innerHTML = '<p>Nessun biglietto trovato.</p>';
                return;
            }

            // Adattamento per la struttura piatta (senza oggetti annidati)
            const enhancedData = data.map(p => ({
                'ID Prenotazione': p.id,
                'Cliente': p.clienteNome ? `${p.clienteNome} ${p.clienteCognome}` : p.clienteId,
                'Destinazione': p.destinazione || 'N/D',
                'Data partenza': p.dataInizio ? p.dataInizio.split('T')[0] : '',
                'Data rientro': p.dataFine ? p.dataFine.split('T')[0] : '',
                'Stato': p.stato,
                'Data prenotazione': p.dataPrenotazione ? p.dataPrenotazione.split('T')[0] : ''
            }));

            const table = buildTableFromData(enhancedData);
            contentDiv.innerHTML = '';
            contentDiv.appendChild(table);

        } catch (error) {
            console.error('Errore loadTickets:', error);
            contentDiv.innerHTML = `<p style="color:red;">Errore di connessione: ${error.message}</p>`;
        }
    }

    // ------------------------------
    // Carica le statistiche all'avvio
    // ------------------------------
    loadStats();
});