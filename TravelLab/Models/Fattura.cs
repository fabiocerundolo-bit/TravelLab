namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_fatture")]
public class Fattura
{
    [Column("id_fattura")]
    public int Id { get; set; }

    [Column("fk_prenotazione_f")]
    public int PrenotazioneId { get; set; }
    public Prenotazione Prenotazione { get; set; }

    [Column("data_emissione",TypeName = "timestamp without time zone")]
    public DateTime DataEmissione { get; set; }

    [Column("importo_totale")]
    public decimal ImportoTotale { get; set; }

    [Column("metodo_pagamento")]
    public string MetodoPagamento { get; set; }
}