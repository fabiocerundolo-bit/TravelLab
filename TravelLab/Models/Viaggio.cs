namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_viaggi")]
public class Viaggio
{
    [Column("id_viaggio")]
    public int Id { get; set; }

    [Column("descrizione")]
    public string Descrizione { get; set; }

    [Column("data_inizio",TypeName = "timestamp without time zone")]
    public DateTime DataInizio { get; set; }

    [Column("data_fine",TypeName = "timestamp without time zone")]
    public DateTime DataFine { get; set; }

    [Column("destinazione")]
    public string Destinazione { get; set; }

    [Column("prezzo_base")]
    public decimal PrezzoBase { get; set; }
    
}