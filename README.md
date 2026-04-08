# TravelLab – Documentazione di progetto (versione 2.0)

## 1. Introduzione

TravelLab è una piattaforma web per un’agenzia di viaggi che offre:

- **Dashboard amministrativa** protetta da login, per la gestione completa di clienti, viaggi, prenotazioni, voli, treni, navi e statistiche.
- **Sito pubblico "The Editorial Voyager"** per la consultazione di destinazioni, offerte e la prenotazione di viaggi con selezione opzionale di treni e navi (per compagnia).

**Tecnologie:**  
Backend: C# / ASP.NET Core 8, Entity Framework Core, PostgreSQL, ASP.NET Core Identity.  
Frontend: HTML5, CSS3 (Tailwind CSS), JavaScript vanilla.

---

## 2. Architettura e sicurezza

### 2.1 Autenticazione e autorizzazione

- **ASP.NET Core Identity** gestisce utenti, ruoli e password (hashing).
- Tabelle Identity: `AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, ecc.
- Due ruoli predefiniti: `Admin` (pieni poteri) e `Operatore` (accesso in sola lettura ad alcune sezioni).
- Utente amministratore di default creato al primo avvio:  
  `username: admin`, `password: Admin@123`.

### 2.2 Separazione API pubbliche e private

- **API pubbliche** (accessibili senza autenticazione):  
  - `GET /api/viaggi` – elenco viaggi  
  - `GET /api/treni` – elenco treni (solo compagnie)  
  - `GET /api/navi` – elenco navi (solo compagnie)  
  - `POST /api/clienti` – creazione cliente  
  - `GET /api/clienti/by-email` – ricerca cliente per email  
  - `POST /api/prenotazioni` – creazione prenotazione  
  - `POST /api/biglietti/treno` – creazione biglietto treno  
  - `POST /api/biglietti/nave` – creazione biglietto nave  

- **API amministrative** (richiedono autenticazione e spesso ruolo `Admin`):  
  - `GET /api/clienti` – elenco completo clienti  
  - `GET /api/clienti/senza-prenotazioni`  
  - `GET /api/statistiche/*` – top destinazioni, ricavi mensili  
  - `GET /api/prenotazioni` – tutte le prenotazioni  
  - `GET /api/prenotazioni/cliente/{id}` – storico cliente  
  - `GET /api/biglietti` – elenco biglietti  
  - `POST /api/viaggi`, `POST /api/clienti` (operazioni di scrittura – solo Admin)

I controller amministrativi sono marcati con `[Authorize(Roles = "Admin")]` o `[Authorize]`. I metodi pubblici hanno `[AllowAnonymous]`.

---

## 3. Database

Lo schema del database è stato rigenerato tramite **migrazioni EF Core** (invece di script manuali), garantendo coerenza con il modello.  
Le tabelle principali sono: `t_clienti`, `t_viaggi`, `t_prenotazioni`, `t_fatture`, `t_servizi`, `t_voli`, `t_treni`, `t_navi`, `t_hotel`, `t_biglietti`, `t_mezzi`, `t_agenzia`, `t_luoghi`, più le tabelle di Identity.

**Correzione importante:** la tabella delle agenzie si chiama `t_agenzia` (non `t_angenzia` come in una versione precedente).

---

## 4. Backend – Controller principali

### 4.1 `AccountController` (autenticazione)

- `POST /api/account/login` – accede, imposta cookie.
- `POST /api/account/logout` – termina sessione.
- `POST /api/account/register` – crea nuovo utente con ruolo `Operatore`.
- `GET /api/account/check` – verifica stato autenticazione.

### 4.2 `ClientiController`

- `GET /api/clienti` → solo Admin.
- `GET /api/clienti/senza-prenotazioni` → solo Admin.
- `POST /api/clienti` → pubblico (creazione da booking).
- `GET /api/clienti/by-email` → pubblico (ricerca per email).

### 4.3 `PrenotazioniController`

- `GET /api/prenotazioni/cliente/{id}` → solo Admin.
- `GET /api/prenotazioni/count` → solo Admin.
- `POST /api/prenotazioni` → pubblico (creazione da booking).
- `GET /api/prenotazioni` → solo Admin (elenco completo).

### 4.4 `ViaggiController`, `TreniController`, `NaviController`

- GET pubblici, nessuna autenticazione.
- I controller di treni e navi restituiscono solo i campi essenziali e gestiscono i valori nulli (per `Mezzo` e `Servizio`).

### 4.5 `BigliettiController`

- `POST /api/biglietti/treno` e `/api/biglietti/nave` → pubblici (usati dal booking).
- `GET /api/biglietti` → solo Admin, con arrotondamento del prezzo.

### 4.6 `StatisticheController`

- `GET /api/statistiche/top-destinazioni` e `ricavi-mensili` → solo Admin.

---

## 5. Frontend

### 5.1 Dashboard amministrativa (`admin.html` + `script.js`)

- Richiede login; reindirizza a `login.html` se non autenticato.
- Tutte le fetch includono `credentials: 'include'`.
- Pulsante di logout.
- Form per inserire clienti, viaggi, prenotazioni (modale).
- Visualizzazione di clienti, viaggi, prenotazioni, voli, treni, navi, statistiche.
- Le date sono formattate senza parte oraria.

### 5.2 Sito pubblico

- **`index.html`**, **`destinations.html`**, **`deals.html`** – visualizzano dati da API pubbliche.
- **`booking.html`** – form di prenotazione con:
  - Selezione destinazione (viaggio) da API `/api/viaggi`.
  - Selezione **compagnia** per treno (elenco unico per compagnia) e per nave (stessa logica).  
    Dietro le quinte, per ogni compagnia viene selezionato il primo treno/nave disponibile e creato il biglietto.
  - Invio: crea cliente (se non esiste), prenotazione e, opzionalmente, biglietti treno e nave.
- **`about.html`** – statico.

---

## 6. Miglioramenti recenti

- **Autenticazione completa** con Identity e ruoli.
- **Separazione netta tra API pubbliche e amministrative**.
- **Raggruppamento di treni e navi per compagnia** nel form di booking.
- **Correzione della gestione dei nulli** in `NaviController` e `TreniController`.
- **Aggiunta di endpoint per la creazione di biglietti** dal booking.
- **Refactoring della pagina di login** e gestione della sessione.
- **Ricreazione del database con migrazioni** (invece di script manuali).

---

## 7. Installazione e avvio (aggiornato)

### Prerequisiti

- .NET SDK 8
- PostgreSQL (locale o remoto)

### Passi

1. Clona il repository.
2. Modifica la stringa di connessione in `appsettings.json`.
3. Esegui le migrazioni:
   ```bash
   dotnet ef database update
   ```
4. Avvia l’applicazione:
   ```bash
   dotnet run
   ```
5. Accedi alla dashboard su `http://localhost:5070/login.html` con `admin` / `Admin@123`.
6. Il sito pubblico è su `http://localhost:5070/`.

---

## 8. Possibili sviluppi futuri

- Aggiungere CAPTCHA sulla creazione clienti (per evitare spam).
- Limitare le informazioni restituite da `GetClienteByEmail` (solo booleano).
- Implementare rate limiting.
- Aggiungere paginazione su tabelle con molti dati.

---

## 9. Conclusione

TravelLab è ora un sistema completo, sicuro e manutenibile, con una chiara separazione tra area pubblica e amministrativa, autenticazione robusta e un’interfaccia utente moderna. Le ultime modifiche hanno risolto i problemi di autorizzazione e migliorato l’esperienza di prenotazione.

---

*Documento aggiornato all’8 aprile 2026.*
