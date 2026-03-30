namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_biglietti")]
public class Biglietto
{
    [Column("id_biglietto")]
    public int Id { get; set; }

    [Column("fk_prenotazione_b")]
    public int PrenotazioneId { get; set; }
    public Prenotazione Prenotazione { get; set; }

    [Column("fk_servizio")]
    public int ServizioId { get; set; }
    public Servizio Servizio { get; set; }

    [Column("prezzo_effettivo")]
    public decimal PrezzoEffettivo { get; set; }
}