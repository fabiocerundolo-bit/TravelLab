-- =====================================================
-- SCRIPT COMPLETO PER RICREARE IL DATABASE TRAVELAB
-- ORDINE CORRETTO: PRIMA MEZZI, POI SERVIZI, POI DETTAGLI
-- =====================================================

DROP TABLE IF EXISTS t_biglietti, t_fatture, t_prenotazioni, t_tratte, t_viaggi,
                     t_voli, t_treni, t_navi, t_servizi, t_mezzi, t_luoghi,
                     t_hotel, t_clienti, t_agenzia CASCADE;

-- =====================================================
-- CREAZIONE TABELLE
-- =====================================================

CREATE TABLE t_luoghi (
    id_luogo SERIAL PRIMARY KEY,
    citta VARCHAR(100),
    cap VARCHAR(20),
    tipo VARCHAR(100)
);

CREATE TABLE t_hotel (
    id_hotel SERIAL PRIMARY KEY,
    nome VARCHAR(100),
    indirizzo VARCHAR(100),
    citta VARCHAR(100),
    stelle INT,
    telefono VARCHAR(30),
    fk_servizio INT UNIQUE
);

CREATE TABLE t_clienti (
    id_cliente SERIAL PRIMARY KEY,
    nome VARCHAR(100),
    cognome VARCHAR(100),
    email VARCHAR(100),
    telefono VARCHAR(30),
    indirizzo VARCHAR(100)
);

CREATE TABLE t_agenzia (
    id_agenzia SERIAL PRIMARY KEY,
    nome VARCHAR(100),
    email VARCHAR(100),
    telefono VARCHAR(30),
    indirizzo VARCHAR(100)
);

CREATE TABLE t_viaggi (
    id_viaggio SERIAL PRIMARY KEY,
    descrizione TEXT,
    data_inizio DATE,
    data_fine DATE,
    destinazione VARCHAR(100),
    prezzo_base DECIMAL
);

CREATE TABLE t_mezzi (
    id_mezzo SERIAL PRIMARY KEY,
    tipo_mezzo VARCHAR(100),
    compagnia VARCHAR(100),
    capacita INT
);

CREATE TABLE t_tratte (
    id_tratta SERIAL PRIMARY KEY,
    fk_mezzo INT REFERENCES t_mezzi(id_mezzo) ON DELETE CASCADE,
    id_luogo_partenza INT REFERENCES t_luoghi(id_luogo) ON DELETE CASCADE,
    id_luogo_arrivo INT REFERENCES t_luoghi(id_luogo) ON DELETE CASCADE,
    orario_partenza TIME,
    orario_arrivo TIME
);

CREATE TABLE t_prenotazioni (
    id_prenotazione SERIAL PRIMARY KEY,
    fk_cliente INT REFERENCES t_clienti(id_cliente) ON DELETE CASCADE,
    fk_viaggio INT REFERENCES t_viaggi(id_viaggio) ON DELETE CASCADE,
    fk_agenzia INT REFERENCES t_agenzia(id_agenzia) ON DELETE CASCADE,
    data_prenotazione DATE,
    stato VARCHAR(100)
);

CREATE TABLE t_fatture (
    id_fattura SERIAL PRIMARY KEY,
    fk_prenotazione_f INT REFERENCES t_prenotazioni(id_prenotazione) ON DELETE CASCADE,
    data_emissione DATE,
    importo_totale DECIMAL,
    metodo_pagamento VARCHAR(100)
);

CREATE TABLE t_servizi (
    id_servizio SERIAL PRIMARY KEY,
    tipo_servizio VARCHAR(20) NOT NULL,
    prezzo_base DECIMAL(10,2)
);

CREATE TABLE t_voli (
    id_servizio INT PRIMARY KEY REFERENCES t_servizi(id_servizio) ON DELETE CASCADE,
    numero_volo VARCHAR(20) NOT NULL,
    compagnia_aerea VARCHAR(100),
    gate VARCHAR(10),
    fk_mezzo INT REFERENCES t_mezzi(id_mezzo) ON DELETE SET NULL
);

CREATE TABLE t_treni (
    id_servizio INT PRIMARY KEY REFERENCES t_servizi(id_servizio) ON DELETE CASCADE,
    numero_treno VARCHAR(20) NOT NULL,
    tipo_treno VARCHAR(50),
    fk_mezzo INT REFERENCES t_mezzi(id_mezzo) ON DELETE SET NULL
);

CREATE TABLE t_navi (
    id_servizio INT PRIMARY KEY REFERENCES t_servizi(id_servizio) ON DELETE CASCADE,
    nome_nave VARCHAR(100) NOT NULL,
    fk_mezzo INT REFERENCES t_mezzi(id_mezzo) ON DELETE SET NULL
);

CREATE TABLE t_biglietti (
    id_biglietto SERIAL PRIMARY KEY,
    fk_prenotazione_b INT REFERENCES t_prenotazioni(id_prenotazione) ON DELETE CASCADE,
    fk_servizio INT REFERENCES t_servizi(id_servizio) ON DELETE CASCADE,
    prezzo_effettivo DECIMAL(10,2)
);

-- Indici
CREATE INDEX idx_viaggi_data_inizio ON t_viaggi(data_inizio);
CREATE INDEX idx_prenotazioni_data ON t_prenotazioni(data_prenotazione);
CREATE INDEX idx_fatture_data ON t_fatture(data_emissione);

-- =====================================================
-- INSERIMENTO DATI (NELL'ORDINE CORRETTO)
-- =====================================================

-- 1. Luoghi
INSERT INTO t_luoghi (citta, cap, tipo) VALUES
('Roma', '00100', 'Città'),
('Milano', '20100', 'Città'),
('Venezia', '30100', 'Città'),
('Napoli', '80100', 'Città'),
('Firenze', '50100', 'Città'),
('Parigi', '75000', 'Capitale'),
('Barcellona', '08000', 'Città'),
('Amsterdam', '1000', 'Capitale'),
('Londra', 'SW1A', 'Capitale'),
('Berlino', '10115', 'Capitale'),
('Madrid', '28001', 'Capitale'),
('Lisbona', '1100', 'Capitale'),
('Atene', '10554', 'Città'),
('Praga', '11000', 'Città'),
('Budapest', '1011', 'Città'),
('Vienna', '1010', 'Città');

-- 2. Hotel (senza fk_servizio, verrà aggiornato dopo)
INSERT INTO t_hotel (nome, indirizzo, citta, stelle, telefono) VALUES
('Hotel Roma Centro', 'Via Nazionale 10', 'Roma', 4, '+39 06 1111111'),
('Grand Hotel Plaza', 'Via del Corso 126', 'Roma', 5, '+39 06 2222222'),
('Hotel Milano Scala', 'Via dell''Orso 7', 'Milano', 4, '+39 02 1111111'),
('Bulgari Hotel Milano', 'Via Privata Fratelli Gabba 7b', 'Milano', 5, '+39 02 2222222'),
('Hotel Danieli', 'Riva degli Schiavoni 4196', 'Venezia', 5, '+39 041 1111111'),
('Hotel Cipriani', 'Giudecca 10', 'Venezia', 5, '+39 041 2222222'),
('Grand Hotel Vesuvio', 'Via Partenope 45', 'Napoli', 4, '+39 081 1111111'),
('Hotel Savoy', 'Piazza della Repubblica 7', 'Firenze', 5, '+39 055 2222222'),
('Hotel Ritz', '15 Place Vendôme', 'Parigi', 5, '+33 1 11111111'),
('Hotel Arts', 'Carrer de la Marina 19', 'Barcellona', 5, '+34 93 1111111'),
('Hotel Krasnapolsky', 'Dam 9', 'Amsterdam', 5, '+31 20 1111111'),
('The Ritz London', '150 Piccadilly', 'Londra', 5, '+44 20 1111111');

-- 3. Clienti
INSERT INTO t_clienti (nome, cognome, email, telefono, indirizzo) VALUES
('Marco', 'Rossi', 'marco.rossi@email.it', '+39 333 1111111', 'Via Roma 1, Milano'),
('Laura', 'Bianchi', 'laura.bianchi@email.it', '+39 333 2222222', 'Via Milano 2, Roma'),
('Giuseppe', 'Verdi', 'giuseppe.verdi@email.it', '+39 333 3333333', 'Via Napoli 3, Torino'),
('Anna', 'Ferrari', 'anna.ferrari@email.it', '+39 333 4444444', 'Via Firenze 4, Bologna'),
('Luca', 'Esposito', 'luca.esposito@email.it', '+39 333 5555555', 'Via Venezia 5, Napoli'),
('Francesca', 'Romano', 'francesca.romano@email.it', '+39 333 6666666', 'Via Roma 6, Palermo'),
('Alessandro', 'Gallo', 'alessandro.gallo@email.it', '+39 333 7777777', 'Via Genova 7, Genova'),
('Chiara', 'Conti', 'chiara.conti@email.it', '+39 333 8888888', 'Via Torino 8, Firenze'),
('Davide', 'Mancini', 'davide.mancini@email.it', '+39 333 9999999', 'Via Bologna 9, Venezia'),
('Elena', 'Giordano', 'elena.giordano@email.it', '+39 333 0000000', 'Via Napoli 10, Roma');

-- 4. Agenzie
INSERT INTO t_agenzia (nome, email, telefono, indirizzo) VALUES
('Viaggi del Sole', 'info@viaggidelsole.it', '+39 02 1111111', 'Via Dante 10, Milano'),
('TurItalia', 'info@turistalia.it', '+39 06 2222222', 'Via Veneto 20, Roma'),
('EuroTrip Agency', 'info@eurotrip.it', '+39 011 3333333', 'Via Po 30, Torino');

-- 5. Mezzi
INSERT INTO t_mezzi (tipo_mezzo, compagnia, capacita) VALUES
('Aereo', 'Alitalia', 180),
('Aereo', 'RyanAir', 189),
('Aereo', 'EasyJet', 180),
('Treno', 'Trenitalia', 300),
('Treno', 'Italo', 280),
('Nave', 'MSC Crociere', 2000),
('Nave', 'Costa Crociere', 2200);

-- 6. Viaggi
INSERT INTO t_viaggi (descrizione, data_inizio, data_fine, destinazione, prezzo_base) VALUES
('Tour delle città d''arte italiane', '2025-06-01', '2025-06-10', 'Roma', 850.00),
('Weekend a Parigi', '2025-07-15', '2025-07-18', 'Parigi', 650.00),
('Barcellona e dintorni', '2025-08-01', '2025-08-08', 'Barcellona', 780.00),
('Amsterdam canali e musei', '2025-09-01', '2025-09-05', 'Amsterdam', 590.00),
('Londra 5 giorni', '2025-10-01', '2025-10-05', 'Londra', 850.00),
('Crociera Mediterraneo Orientale', '2025-07-15', '2025-07-25', 'Venezia', 1800.00),
('Weekend a Venezia', '2025-07-01', '2025-07-04', 'Venezia', 450.00),
('Napoli e Costiera', '2025-07-10', '2025-07-17', 'Napoli', 790.00);

-- 7. Tratte (collegano mezzi a luoghi)
INSERT INTO t_tratte (fk_mezzo, id_luogo_partenza, id_luogo_arrivo, orario_partenza, orario_arrivo) VALUES
(1, 1, 6, '08:00', '10:30'),
(2, 1, 7, '09:00', '11:00'),
(4, 1, 2, '06:00', '09:00'),
(5, 2, 3, '07:00', '09:30'),
(6, 3, 7, '09:00', '17:00');

-- 8. Prenotazioni
INSERT INTO t_prenotazioni (fk_cliente, fk_viaggio, fk_agenzia, data_prenotazione, stato) VALUES
(1, 1, 1, '2025-04-01', 'Confermata'),
(2, 2, 2, '2025-04-05', 'Confermata'),
(3, 3, 1, '2025-04-10', 'In attesa'),
(4, 4, 3, '2025-04-12', 'Confermata'),
(5, 5, 2, '2025-04-15', 'Annullata'),
(1, 6, 3, '2025-04-18', 'Confermata'),
(2, 7, 1, '2025-04-20', 'In attesa'),
(6, 8, 2, '2025-05-01', 'Confermata');

-- 9. Fatture (solo per prenotazioni confermate)
INSERT INTO t_fatture (fk_prenotazione_f, data_emissione, importo_totale, metodo_pagamento) VALUES
(1, '2025-04-02', 850.00, 'Carta di credito'),
(2, '2025-04-06', 650.00, 'Bonifico'),
(4, '2025-04-13', 590.00, 'Carta di credito'),
(6, '2025-04-19', 1800.00, 'PayPal'),
(8, '2025-05-02', 790.00, 'Carta di credito');

-- =====================================================
-- SERVIZI PER VOLI, TRENI, NAVI (id_servizio = id_mezzo)
-- =====================================================

-- 10. Inserisci servizi per i voli (usi l'id_mezzo come id_servizio)
INSERT INTO t_servizi (id_servizio, tipo_servizio, prezzo_base)
SELECT m.id_mezzo, 'VOLO', 0
FROM t_mezzi m
WHERE m.tipo_mezzo = 'Aereo'
  AND NOT EXISTS (SELECT 1 FROM t_servizi WHERE id_servizio = m.id_mezzo);

-- 11. Inserisci i voli
INSERT INTO t_voli (id_servizio, numero_volo, compagnia_aerea, gate, fk_mezzo)
SELECT m.id_mezzo,
       LEFT(m.compagnia, 2) || LPAD(m.id_mezzo::TEXT, 3, '0'),
       m.compagnia,
       'Gate A',
       m.id_mezzo
FROM t_mezzi m
WHERE m.tipo_mezzo = 'Aereo'
  AND NOT EXISTS (SELECT 1 FROM t_voli WHERE id_servizio = m.id_mezzo);

-- 12. Servizi per treni
INSERT INTO t_servizi (id_servizio, tipo_servizio, prezzo_base)
SELECT m.id_mezzo, 'TRENO', 0
FROM t_mezzi m
WHERE m.tipo_mezzo = 'Treno'
  AND NOT EXISTS (SELECT 1 FROM t_servizi WHERE id_servizio = m.id_mezzo);

-- 13. Inserisci i treni
INSERT INTO t_treni (id_servizio, numero_treno, tipo_treno, fk_mezzo)
SELECT m.id_mezzo,
       m.compagnia || LPAD(m.id_mezzo::TEXT, 3, '0'),
       'Regionale',
       m.id_mezzo
FROM t_mezzi m
WHERE m.tipo_mezzo = 'Treno'
  AND NOT EXISTS (SELECT 1 FROM t_treni WHERE id_servizio = m.id_mezzo);

-- 14. Servizi per navi
INSERT INTO t_servizi (id_servizio, tipo_servizio, prezzo_base)
SELECT m.id_mezzo, 'NAVE', 0
FROM t_mezzi m
WHERE m.tipo_mezzo = 'Nave'
  AND NOT EXISTS (SELECT 1 FROM t_servizi WHERE id_servizio = m.id_mezzo);

-- 15. Inserisci le navi
INSERT INTO t_navi (id_servizio, nome_nave, fk_mezzo)
SELECT m.id_mezzo,
       m.compagnia || ' ' || m.id_mezzo,
       m.id_mezzo
FROM t_mezzi m
WHERE m.tipo_mezzo = 'Nave'
  AND NOT EXISTS (SELECT 1 FROM t_navi WHERE id_servizio = m.id_mezzo);

-- =====================================================
-- SERVIZI PER HOTEL (con id_servizio automatico)
-- =====================================================

-- 16. Crea un servizio per ogni hotel (senza specificare id_servizio)
INSERT INTO t_servizi (tipo_servizio, prezzo_base)
SELECT 'HOTEL', 0 FROM t_hotel;

-- 17. Collega ogni hotel al proprio servizio (associazione per ordine di inserimento)
UPDATE t_hotel h
SET fk_servizio = s.id_servizio
FROM (
    SELECT id_servizio, ROW_NUMBER() OVER (ORDER BY id_servizio) AS rn
    FROM t_servizi
    WHERE tipo_servizio = 'HOTEL'
) s
WHERE h.id_hotel = s.rn;

-- =====================================================
-- BIGLIETTI (ora tutte le FK sono valide)
-- =====================================================

-- 18. Biglietti hotel (60% delle prenotazioni)
INSERT INTO t_biglietti (fk_prenotazione_b, fk_servizio, prezzo_effettivo)
SELECT p.id_prenotazione, h.fk_servizio, 200 + floor(random() * 600)
FROM t_prenotazioni p
CROSS JOIN LATERAL (SELECT fk_servizio FROM t_hotel ORDER BY random() LIMIT 1) h
WHERE random() < 0.6
  AND NOT EXISTS (SELECT 1 FROM t_biglietti b WHERE b.fk_prenotazione_b = p.id_prenotazione);

-- 19. Biglietti volo (50% delle prenotazioni)
INSERT INTO t_biglietti (fk_prenotazione_b, fk_servizio, prezzo_effettivo)
SELECT p.id_prenotazione, v.id_servizio, 150 + floor(random() * 400)
FROM t_prenotazioni p
CROSS JOIN LATERAL (SELECT id_servizio FROM t_voli ORDER BY random() LIMIT 1) v
WHERE random() < 0.5
  AND NOT EXISTS (SELECT 1 FROM t_biglietti b WHERE b.fk_prenotazione_b = p.id_prenotazione AND b.fk_servizio = v.id_servizio);

-- 20. Biglietti treno (30% delle prenotazioni)
INSERT INTO t_biglietti (fk_prenotazione_b, fk_servizio, prezzo_effettivo)
SELECT p.id_prenotazione, t.id_servizio, 50 + floor(random() * 150)
FROM t_prenotazioni p
CROSS JOIN LATERAL (SELECT id_servizio FROM t_treni ORDER BY random() LIMIT 1) t
WHERE random() < 0.3
  AND NOT EXISTS (SELECT 1 FROM t_biglietti b WHERE b.fk_prenotazione_b = p.id_prenotazione AND b.fk_servizio = t.id_servizio);

-- 21. Biglietti nave (20% delle prenotazioni)
INSERT INTO t_biglietti (fk_prenotazione_b, fk_servizio, prezzo_effettivo)
SELECT p.id_prenotazione, n.id_servizio, 200 + floor(random() * 500)
FROM t_prenotazioni p
CROSS JOIN LATERAL (SELECT id_servizio FROM t_navi ORDER BY random() LIMIT 1) n
WHERE random() < 0.2
  AND NOT EXISTS (SELECT 1 FROM t_biglietti b WHERE b.fk_prenotazione_b = p.id_prenotazione AND b.fk_servizio = n.id_servizio);

-- =====================================================
-- VINCOLO FINALE (già creato, ma per sicurezza)
-- =====================================================
ALTER TABLE t_hotel ADD CONSTRAINT fk_hotel_servizio FOREIGN KEY (fk_servizio) REFERENCES t_servizi(id_servizio);

-- =====================================================
-- FINE SCRIPT
-- =====================================================