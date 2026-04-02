# TravelLab – Documentazione di progetto

## 1. Introduzione

TravelLab è una piattaforma web che consente a un’agenzia di viaggi di gestire clienti, pacchetti turistici, prenotazioni, fatture e servizi aggiuntivi (voli, hotel). L’applicazione è composta da:

- **Backend** in **C# con ASP.NET Core 8** e **Entity Framework Core** (ORM per PostgreSQL).
- **Frontend** in **HTML/CSS/JavaScript vanilla** con interfaccia moderna e responsive.
- **Database** **PostgreSQL** con un modello relazionale normalizzato.

L’applicazione espone API RESTful e fornisce una dashboard interattiva per le operazioni quotidiane.

---

## 2. Architettura generale

```
TravelLab/
├── Controllers/          # API endpoint
├── Models/               # Entità del database e DTO
├── Data/                 # DbContext e configurazione EF
├── wwwroot/              # Frontend statico (HTML, CSS, JS)
├── Program.cs            # Configurazione del server
├── appsettings.json      # Configurazioni (stringa di connessione)
└── TravelLab.csproj      # File di progetto
```

**Flusso dei dati:**  
Browser → Frontend (JS) → API (C#) → Database → Risposta JSON → Visualizzazione tabellare.

---

## 3. Database

### 3.1 Modello entità-relazione (sintesi)

Le principali tabelle sono:

| Tabella          | Descrizione |
|------------------|-------------|
| `t_clienti`      | Anagrafica clienti |
| `t_agenzia`      | Agenzie di viaggio (rivenditori) |
| `t_viaggi`       | Pacchetti turistici (destinazione, date, prezzo base) |
| `t_prenotazioni` | Prenotazioni effettuate (collega cliente, viaggio, agenzia) |
| `t_fatture`      | Fatture emesse per prenotazioni confermate |
| `t_luoghi`       | Città, aeroporti, stazioni, porti |
| `t_mezzi`        | Mezzi di trasporto (aereo, treno, nave, autobus) |
| `t_tratte`       | Percorsi con orari (collega mezzi e luoghi) |
| `t_servizi`      | Entità padre per servizi prenotabili (hotel, volo, treno, nave) |
| `t_hotel`        | Dettagli degli hotel (collegati a `t_servizi`) |
| `t_voli`         | Dettagli dei voli (collegati a `t_servizi`) |
| `t_biglietti`    | Associa una prenotazione a un servizio (con prezzo effettivo) |

### 3.2 Relazioni principali

- Una prenotazione appartiene a un cliente, un viaggio e un’agenzia.
- Una prenotazione può avere più biglietti (hotel, volo, ecc.).
- Ogni biglietto si riferisce a un servizio (`t_servizi`), che a sua volta ha dettagli specifici in `t_hotel` o `t_voli`.
- I viaggi hanno un prezzo base; le fatture registrano l’importo totale pagato.

### 3.3 Script di inizializzazione

Il file `init-db/01-schema.sql` (incluso nel repository) crea tutte le tabelle e popola il database con dati di esempio realistici (oltre 50 clienti, 30 hotel, 40 viaggi, 200 prenotazioni, ecc.).

---

## 4. Backend – API REST

### 4.1 Tecnologie

- ASP.NET Core 8
- Entity Framework Core (con Npgsql)
- Autenticazione: nessuna (per sviluppo; estendibile)

### 4.2 Endpoint principali

| Metodo | Endpoint | Descrizione |
|--------|----------|-------------|
| GET    | `/api/clienti` | Elenco di tutti i clienti (solo campi base) |
| POST   | `/api/clienti` | Crea un nuovo cliente |
| GET    | `/api/clienti/senza-prenotazioni` | Clienti senza prenotazioni |
| GET    | `/api/viaggi` | Elenco viaggi (solo campi base) |
| POST   | `/api/viaggi` | Crea un nuovo viaggio |
| GET    | `/api/prenotazioni/cliente/{id}` | Storico prenotazioni di un cliente |
| POST   | `/api/prenotazioni` | Crea una nuova prenotazione (DTO) |
| GET    | `/api/prenotazioni/count` | Numero totale di prenotazioni |
| GET    | `/api/voli` | Cerca voli per destinazione e intervallo date |
| GET    | `/api/agenzie` | Elenco agenzie |
| GET    | `/api/statistiche/top-destinazioni` | Top 10 destinazioni per numero di prenotazioni |
| GET    | `/api/statistiche/ricavi-mensili` | Ricavi totali raggruppati per mese |

Tutti gli endpoint restituiscono JSON. La serializzazione evita cicli grazie all’uso di oggetti anonimi (proiezioni).

### 4.3 Gestione errori

- `400 Bad Request` per validazione fallita o dati mancanti.
- `404 Not Found` quando una risorsa non esiste (gestito nei controller).
- `500 Internal Server Error` per eccezioni non gestite (in sviluppo viene mostrata la pagina di diagnostica).

### 4.4 DTO (Data Transfer Object)

Per la creazione di prenotazioni si utilizza `CreatePrenotazioneDto`:

```csharp
public class CreatePrenotazioneDto
{
    public int ClienteId { get; set; }
    public int ViaggioId { get; set; }
    public int AgenziaId { get; set; }
    public DateTime DataPrenotazione { get; set; }
    public string Stato { get; set; }
}
```

---

## 5. Frontend – Dashboard

### 5.1 Struttura dei file

- `index.html` – layout a due colonne (sidebar + area principale) con modale per i form.
- `style.css` – stili professionali (font Inter, ombre, card, responsive).
- `script.js` – gestione eventi, chiamate fetch, popolamento tabelle e statistiche.

### 5.2 Funzionalità implementate

| Sezione | Azione |
|---------|--------|
| Sidebar | Elenco clienti, storico prenotazioni (con prompt ID), ricerca voli, top destinazioni, ricavi mensili, clienti senza prenotazioni. |
| Form | Nuovo cliente, nuovo viaggio, nuova prenotazione (con select popolate dinamicamente). |
| Dashboard | Tre card statistiche (numero clienti, numero prenotazioni, ricavi totali) aggiornate automaticamente. |

### 5.3 Comunicazione con il backend

Tutte le richieste sono effettuate con `fetch()`. Le risposte JSON vengono trasformate in tabelle HTML tramite la funzione `buildTableFromData`. I form inviano dati in formato JSON con metodo `POST`.

### 5.4 Gestione modale

I form compaiono in un overlay modale quando si clicca sui pulsanti dedicati. La chiusura avviene tramite il tasto `X` o dopo il submit riuscito.

---

## 6. Installazione e avvio

### 6.1 Prerequisiti

- **.NET SDK 8** (o versione compatibile)
- **PostgreSQL** (locale o remoto) – versione 15 o superiore
- **Node.js** (non necessario, solo per il frontend statico)

### 6.2 Clonare il repository

```bash
git clone https://github.com/fabiocerundolo-bit/TravelLab.git
cd TravelLab
```

### 6.3 Configurare il database

1. Creare un database `sistema_viaggi` (o altro nome) su PostgreSQL.
2. Eseguire lo script `init-db/01-schema.sql` per creare le tabelle e i dati di esempio.
3. Modificare la stringa di connessione in `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=sistema_viaggi;Username=postgres;Password=tuapassword"
}
```

### 6.4 Avviare l’applicazione

```bash
dotnet restore
dotnet build
dotnet run
```

L’applicazione sarà disponibile su `http://localhost:5070` (la porta può variare).

### 6.5 Utilizzo

Aprire il browser all’indirizzo indicato. La dashboard è pronta per essere utilizzata.

---

## 7. Possibili estensioni future

- **Autenticazione** (JWT o Identity) per proteggere le API.
- **Grafici** (Chart.js) per visualizzare l’andamento dei ricavi.
- **Paginazione** su tabelle con molti dati.
- **Gestione di treni e navi** (già prevista nelle tabelle ma non implementata nel frontend).
- **Deploy** su cloud (Azure, AWS, Railway) con container Docker.

---

## 8. Conclusioni

TravelLab è un progetto full‑stack funzionante che dimostra l’integrazione tra C#, Entity Framework, PostgreSQL e un frontend vanilla. Offre una base solida per un’agenzia di viaggi reale, con la possibilità di essere facilmente esteso e personalizzato.

---

*Documento redatto per il progetto TravelLab – versione 1.0*
