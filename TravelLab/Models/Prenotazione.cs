namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;


[Table("t_prenotazioni")]
public class Prenotazione
{
    [Column("id_prenotazione")]
    public int Id { get; set; }

    [Column("fk_cliente")]
    public int ClienteId { get; set; }
    public Cliente Cliente { get; set; }

    [Column("fk_viaggio")]
    public int ViaggioId { get; set; }
    public Viaggio Viaggio { get; set; }

    [Column("fk_agenzia")]
    public int AgenziaId { get; set; }

    [Column("data_prenotazione", TypeName = "timestamp without time zone")]
    public DateTime DataPrenotazione { get; set; }

    [Column("stato")]
    public string Stato { get; set; }

    public Fattura Fattura { get; set; }
    public ICollection<Biglietto> Biglietti { get; set; }
}