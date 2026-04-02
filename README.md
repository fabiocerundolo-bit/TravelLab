# TravelLab Documentazione di progetto (versione integrata)

## 1. Introduzione

TravelLab è una piattaforma web completa per un?agenzia di viaggi. Offre due interfacce:

- **Dashboard amministrativa** (accessibile su `/admin.html`) ? per gestire clienti, viaggi, prenotazioni, visualizzare statistiche.
- **Sito pubblico "The Editorial Voyager"** (accessibile su `/index.html`) ? per presentare destinazioni, offerte e permettere ai clienti di prenotare viaggi.

Il backend è realizzato in **C# con ASP.NET Core 8** e **Entity Framework Core** su database **PostgreSQL**. Il frontend utilizza **HTML/CSS/JavaScript vanilla** con **Tailwind CSS** per lo stile.

---

## 2. Architettura generale

```
TravelLab/
??? Controllers/               # API REST
??? Models/                    # Entit� e DTO
??? Data/                      # DbContext
??? wwwroot/                   # File statici (frontend)
?   ??? admin.html             # Dashboard amministrativa
?   ??? index.html             # Homepage pubblica
?   ??? destinations.html      # Elenco destinazioni
?   ??? deals.html             # Offerte
?   ??? booking.html           # Form di prenotazione
?   ??? about.html             # Chi siamo
?   ??? styles.css             # Stili personalizzati
?   ??? tailwind.config.js     # Configurazione Tailwind (non incluso nel browser)
??? Program.cs
??? appsettings.json
```

**Flusso dati:**  
Browser ? Frontend (fetch) ? API ? Database ? Risposta JSON ? Renderizzazione.

---

## 3. Database

Il database � descritto dallo script SQL `schema.sql` (presente nel repository). Le principali tabelle sono:

| Tabella          | Descrizione |
|------------------|-------------|
| `t_clienti`      | Clienti (anagrafica) |
| `t_agenzia`      | Agenzie (rivenditori) |
| `t_viaggi`       | Pacchetti turistici |
| `t_prenotazioni` | Prenotazioni effettuate |
| `t_fatture`      | Fatture |
| `t_luoghi`       | Citt�, aeroporti, etc. |
| `t_mezzi`        | Mezzi di trasporto |
| `t_tratte`       | Percorsi e orari |
| `t_servizi`      | Servizi aggiuntivi (hotel, voli) |
| `t_hotel`, `t_voli` | Dettagli specifici |
| `t_biglietti`    | Associazione prenotazione-servizio |

**Nota:** Le colonne di tipo data sono `timestamp without time zone` per evitare problemi di fuso orario.

---

## 4. Backend ? API REST

### Endpoint principali

| Metodo | Endpoint | Descrizione | Utilizzato da |
|--------|----------|-------------|----------------|
| GET | `/api/clienti` | Elenco clienti (campi base) | Dashboard, booking |
| POST | `/api/clienti` | Crea nuovo cliente | Booking, dashboard |
| GET | `/api/clienti/by-email` | Cerca cliente per email | Booking |
| GET | `/api/clienti/senza-prenotazioni` | Clienti senza prenotazioni | Dashboard |
| GET | `/api/viaggi` | Elenco viaggi | Sito pubblico, dashboard |
| POST | `/api/viaggi` | Crea nuovo viaggio | Dashboard |
| GET | `/api/prenotazioni/cliente/{id}` | Storico prenotazioni cliente | Dashboard |
| POST | `/api/prenotazioni` | Crea prenotazione | Booking, dashboard |
| GET | `/api/prenotazioni/count` | Numero totale prenotazioni | Dashboard (statistiche) |
| GET | `/api/voli` | Ricerca voli per destinazione/intervallo | Dashboard |
| GET | `/api/agenzie` | Elenco agenzie | Dashboard (popolamento select) |
| GET | `/api/statistiche/top-destinazioni` | Top 10 destinazioni | Dashboard |
| GET | `/api/statistiche/ricavi-mensili` | Ricavi mensili | Dashboard |

### DTO utilizzati

- `CreatePrenotazioneDto` ? per ricevere i dati di una nuova prenotazione senza cicli di navigazione.

### Gestione errori

- La serializzazione JSON utilizza **oggetti anonimi** per evitare cicli di riferimento.
- Tutti gli endpoint restituiscono JSON validi (anche `null`).

---

## 5. Frontend

### 5.1 Dashboard amministrativa (`admin.html`)

- **Layout**: sidebar + area principale con card statistiche.
- **Funzionalità**:
    - Visualizzazione clienti, viaggi, prenotazioni.
    - Creazione di nuovi clienti, viaggi, prenotazioni.
    - Ricerca voli per destinazione e date.
    - Top destinazioni e ricavi mensili.
    - Clienti senza prenotazioni.
- **Comunicazione**: chiamate fetch alle API con aggiornamento dinamico delle tabelle.
- **Statistiche**: conteggio clienti, prenotazioni, ricavi totali (da endpoint dedicati).

### 5.2 Sito pubblico "The Editorial Voyager"

Il sito è composto da pagine statiche ma **dinamiche** grazie alle chiamate AJAX alle API.

#### Pagine:

- **`index.html`** ? Homepage:
    - Hero section con ricerca (statica).
    - Sezione "Featured Signature Journeys" (primi 3 viaggi dal DB).
    - Griglia "Explore the Horizon" (prime 4 destinazioni uniche).
- **`destinations.html`** ? Elenco completo di tutti i viaggi, con card che mostrano destinazione, descrizione e prezzo.
- **`deals.html`** ? Offerte: mostra i 6 viaggi pi� economici, applicando uno sconto casuale (10-40%) e visualizzando il prezzo scontato.
- **`booking.html`** ? Form di prenotazione:
    - Popola il select delle destinazioni dai viaggi reali.
    - All?invio, cerca o crea il cliente (per email) e crea una prenotazione (con agenzia fissa id=1).
- **`about.html`** ? Contenuto statico (chi siamo, team, sostenibilit�).

#### Note tecniche:

- Le immagini sono placeholder di Unsplash (possono essere sostituite).
- Il file `tailwind.config.js` **non deve essere incluso** nelle pagine (solo il CDN Tailwind).
- Tutte le pagine condividono lo stesso `styles.css` per personalizzazioni aggiuntive.

### 5.3 Gestione della navigazione

- Il logo "The Editorial Voyager" punta a `index.html` (homepage pubblica).
- La dashboard amministrativa � stata rinominata in `admin.html` per evitare conflitti.
- I link nel menu superiore collegano correttamente le pagine tra loro.

---

## 6. Installazione e avvio

### Prerequisiti

- .NET SDK 8
- PostgreSQL (locale o remoto)
- (Opzionale) Node.js ? non necessario, solo per sviluppo Tailwind

### Passi

1. **Clonare il repository** (o copiare i file).
2. **Creare il database** `sistema_viaggi` ed eseguire lo script `schema.sql` (struttura + dati).
3. **Configurare la stringa di connessione** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=sistema_viaggi;Username=postgres;Password=tuapassword"
   }
   ```
4. **Aggiungere l?endpoint `by-email`** nel controller `ClientiController` (se non presente).
5. **Assicurarsi che esista un?agenzia con id=1** nel database (eseguire la INSERT se necessario).
6. **Copiare tutti i file HTML/CSS/JS nella cartella `wwwroot`** (sostituendo eventuali file esistenti).
7. **Ricostruire e avviare**:
   ```bash
   dotnet build
   dotnet run
   ```
8. **Accedere**:
    - Sito pubblico: `http://localhost:5070/`
    - Dashboard admin: `http://localhost:5070/admin.html`

---

## 7. Risoluzione dei problemi comuni

| Problema | Soluzione |
|----------|-----------|
| `Unexpected end of JSON input` | Verificare che l?endpoint `/api/clienti/by-email` esista e che la risposta sia JSON. Aggiungere il metodo nel controller. |
| `tailwind is not defined` | Rimuovere lo script `tailwind.config.js` dalle pagine HTML (non serve nel browser). |
| La home page mostra la dashboard | Rinominare la dashboard in `admin.html` e lasciare il nuovo `index.html` per il sito pubblico. |
| Il booking fallisce perch� agenzia non trovata | Assicurarsi che esista un record in `t_agenzia` con `id_agenzia = 1`. |
| Le immagini non vengono caricate | Le immagini sono placeholder di Unsplash; funzionano online. In locale possono essere sostituite con URL validi. |

---

## 8. Possibili estensioni future

- **Autenticazione** per distinguere admin da utenti pubblici.
- **Paginazione** su tabelle con molti dati.
- **Caricamento di immagini** reali per ogni destinazione (da salvare nel DB o in cloud storage).
- **Filtri** sulla pagina delle destinazioni (per continente, prezzo, etc.).
- **Gestione completa di voli e hotel** nel sito pubblico (attualmente solo viaggi).

---

## 9. Conclusione

TravelLab è ora un progetto full?stack completo, con due interfacce distinte (admin e pubblica) che condividono lo stesso database e le stesse API. Il sito pubblico "The Editorial Voyager" � completamente integrato e permette ai visitatori di esplorare le destinazioni e prenotare viaggi in modo dinamico.

La documentazione è allineata con lo stato attuale del progetto e può essere utilizzata per manutenzione o ulteriori sviluppi.

---

*Documento aggiornato al 2 aprile 2026.*
