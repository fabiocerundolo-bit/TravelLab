# TravelLab – Documentazione di progetto (versione integrata)

## 1. Introduzione

TravelLab è una piattaforma web completa per un’agenzia di viaggi. Offre due interfacce:

- **Dashboard amministrativa** – per gestire clienti, viaggi, prenotazioni, visualizzare statistiche, voli, treni e navi.
- **Sito pubblico "The Editorial Voyager"** – per presentare destinazioni, offerte e permettere ai clienti di prenotare viaggi, selezionando opzionalmente treni e navi.

**Tecnologie utilizzate:**

- Backend: C# / ASP.NET Core 8, Entity Framework Core, PostgreSQL
- Frontend: HTML5, CSS3 (Tailwind CSS), JavaScript vanilla
- API: RESTful con JSON
- Strumenti: Visual Studio / Rider, DataGrip, Git

---

## 2. Architettura generale

```
TravelLab/
├── Controllers/               # API REST
│   ├── ClientiController.cs
│   ├── ViaggiController.cs
│   ├── PrenotazioniController.cs
│   ├── VoliController.cs
│   ├── TreniController.cs
│   ├── NaviController.cs
│   ├── BigliettiController.cs
│   ├── StatisticheController.cs
│   └── AgenzieController.cs
├── Models/                    # Entità e DTO
│   ├── Cliente.cs
│   ├── Viaggio.cs
│   ├── Prenotazione.cs
│   ├── Fattura.cs
│   ├── Servizio.cs
│   ├── Volo.cs
│   ├── Treno.cs
│   ├── Nave.cs
│   ├── Hotel.cs
│   ├── Biglietto.cs
│   ├── Mezzo.cs
│   └── ...
├── Data/                      # DbContext (TravelLabContext)
├── wwwroot/                   # File statici (frontend)
│   ├── admin.html             # Dashboard amministrativa
│   ├── index.html             # Homepage pubblica
│   ├── destinations.html      # Elenco destinazioni
│   ├── deals.html             # Offerte
│   ├── booking.html           # Form di prenotazione (con treni/navi)
│   ├── about.html             # Chi siamo
│   ├── styles.css / styles1.css
│   └── script.js              # Logica dashboard
├── Program.cs
└── appsettings.json
```

**Flusso dati:**  
Browser → Frontend (fetch) → API → Database → Risposta JSON → Renderizzazione.

---

## 3. Database

### 3.1 Modello ER (sintesi)

Le principali tabelle sono:

| Tabella          | Descrizione |
|------------------|-------------|
| `t_clienti`      | Clienti (anagrafica) |
| `t_agenzia`      | Agenzie (rivenditori) |
| `t_viaggi`       | Pacchetti turistici |
| `t_prenotazioni` | Prenotazioni effettuate |
| `t_fatture`      | Fatture |
| `t_luoghi`       | Città, aeroporti, etc. |
| `t_mezzi`        | Mezzi di trasporto (aereo, treno, nave, autobus) |
| `t_tratte`       | Percorsi e orari |
| `t_servizi`      | Servizi aggiuntivi (hotel, volo, treno, nave) – pattern di ereditarietà |
| `t_hotel`        | Dettagli hotel (collegato a `t_servizi`) |
| `t_voli`         | Dettagli voli (collegato a `t_servizi`) |
| `t_treni`        | Dettagli treni (collegato a `t_servizi`) |
| `t_navi`         | Dettagli navi (collegato a `t_servizi`) |
| `t_biglietti`    | Associa una prenotazione a un servizio (con prezzo effettivo) |

**Relazioni principali:**  
- Una prenotazione appartiene a un cliente, un viaggio e un’agenzia.  
- Una prenotazione può avere più biglietti.  
- Ogni biglietto si riferisce a un servizio (`t_servizi`), che a sua volta ha dettagli specifici in `t_hotel`, `t_voli`, `t_treni` o `t_navi`.  
- I mezzi (`t_mezzi`) sono collegati ai servizi di trasporto (`t_voli`, `t_treni`, `t_navi`) tramite `fk_mezzo`.

### 3.2 Script di inizializzazione

Il file `init-db/01-schema.sql` crea tutte le tabelle e popola il database con dati di esempio (clienti, hotel, viaggi, prenotazioni, voli, treni, navi, biglietti). Lo script è idempotente (può essere eseguito più volte).

---

## 4. Backend – API REST

### 4.1 Endpoint principali

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
| GET | `/api/treni` | Elenco treni (con dettagli servizio) | Dashboard, booking |
| GET | `/api/navi` | Elenco navi | Dashboard, booking |
| POST | `/api/biglietti/treno` | Crea biglietto per un treno | Booking |
| POST | `/api/biglietti/nave` | Crea biglietto per una nave | Booking |
| GET | `/api/agenzie` | Elenco agenzie | Dashboard (popolamento select) |
| GET | `/api/statistiche/top-destinazioni` | Top 10 destinazioni | Dashboard |
| GET | `/api/statistiche/ricavi-mensili` | Ricavi mensili | Dashboard |

### 4.2 DTO utilizzati

- `CreatePrenotazioneDto` – per la creazione di una prenotazione (solo ID, data, stato).
- `CreateBigliettoTrenoDto` – per la creazione di un biglietto treno.
- `CreateBigliettoNaveDto` – per la creazione di un biglietto nave.

### 4.3 Gestione errori

- La serializzazione JSON utilizza **oggetti anonimi** per evitare cicli di riferimento.
- Tutti gli endpoint restituiscono JSON validi (anche `null`).

---

## 5. Frontend

### 5.1 Dashboard amministrativa (`admin.html` + `script.js`)

- **Layout**: sidebar + area principale con card statistiche.
- **Funzionalità**:
  - Visualizzazione di clienti, viaggi, prenotazioni, voli, **treni**, **navi**.
  - Creazione di nuovi clienti, viaggi, prenotazioni.
  - Ricerca voli per destinazione e date.
  - Top destinazioni e ricavi mensili.
  - Clienti senza prenotazioni.
- **Comunicazione**: chiamate fetch alle API con aggiornamento dinamico delle tabelle.
- **Statistiche**: conteggio clienti, prenotazioni, ricavi totali (da endpoint dedicati).

### 5.2 Sito pubblico "The Editorial Voyager"

#### Pagine dinamiche:

- **`index.html`** – Homepage: viaggi in evidenza, destinazioni principali (chiamate a `/api/viaggi`).
- **`destinations.html`** – Elenco completo di tutti i viaggi.
- **`deals.html`** – Offerte (viaggi più economici con sconto simulato).
- **`booking.html`** – Form di prenotazione:
  - Popola select destinazioni da `/api/viaggi`.
  - Popola select treni da `/api/treni` e navi da `/api/navi`.
  - All’invio, cerca o crea il cliente (per email), crea la prenotazione e, se selezionati, crea i biglietti per treno e nave tramite gli endpoint dedicati.
- **`about.html`** – Contenuto statico.

#### Note tecniche:

- Le immagini sono placeholder di Unsplash (sostituibili).
- Tutte le pagine condividono lo stesso foglio di stile (`styles1.css`).
- La formattazione delle date (rimozione della parte oraria) è gestita nel JavaScript.

---

## 6. Installazione e avvio

### Prerequisiti

- .NET SDK 8
- PostgreSQL (locale o remoto) – versione 15 o superiore
- (Opzionale) Node.js – solo per sviluppo Tailwind

### Passi

1. **Clonare il repository** (o copiare i file).
2. **Creare il database** `sistema_viaggi` ed eseguire lo script `schema.sql` (struttura + dati).
3. **Configurare la stringa di connessione** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Host=localhost;Port=5432;Database=sistema_viaggi;Username=postgres;Password=tuapassword"
   }
   ```
4. **Aggiungere eventuali endpoint mancanti** (tutti quelli elencati sono già implementati).
5. **Assicurarsi che esista un’agenzia con `id=1`** nel database (per il booking).
6. **Copiare tutti i file HTML/CSS/JS nella cartella `wwwroot`**.
7. **Ricostruire e avviare**:
   ```bash
   dotnet build
   dotnet run
   ```
8. **Accedere**:
   - Sito pubblico: `http://localhost:5070/`
   - Dashboard admin: `http://localhost:5070/admin.html`

---

## 7. Estensioni recenti (treni e navi)

- **Database**: aggiunte tabelle `t_treni` e `t_navi`, popolate con dati di esempio.
- **Backend**: creati controller `TreniController`, `NaviController` e metodi per la creazione di biglietti treno/nave in `BigliettiController`.
- **Frontend dashboard**: aggiunti pulsanti “Treni” e “Navi” per visualizzare i dati.
- **Frontend pubblico**: il form di booking (`booking.html`) ora permette di selezionare un treno e una nave opzionali; i biglietti corrispondenti vengono creati automaticamente dopo la prenotazione.

---

## 8. Possibili estensioni future

- **Autenticazione** (JWT o Identity) per distinguere admin da utenti pubblici.
- **Paginazione** su tabelle con molti dati.
- **Caricamento di immagini** reali per ogni destinazione.
- **Filtri** sulla pagina delle destinazioni (per continente, prezzo, etc.).
- **Grafici** per l’andamento dei ricavi.
- **Deploy** su cloud (Azure, AWS, Railway) con container Docker.

---

## 9. Conclusione

TravelLab è ora un progetto full‑stack completo e moderno, con due interfacce distinte (admin e pubblica) che condividono lo stesso database e le stesse API. Il sito pubblico “The Editorial Voyager” è completamente integrato e permette ai visitatori di esplorare le destinazioni, prenotare viaggi e aggiungere servizi di trasporto (treni e navi) in modo dinamico.

La documentazione è allineata con lo stato attuale del progetto e può essere utilizzata per manutenzione o ulteriori sviluppi.

---

*Documento aggiornato al 5 aprile 2026.*
