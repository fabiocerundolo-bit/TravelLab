using System.ComponentModel.DataAnnotations;

namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_hotel")]
public class Hotel
{
    [Key]
    [Column("id_hotel")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("indirizzo")]
    public string Indirizzo { get; set; }

    [Column("citta")]
    public string Citta { get; set; }

    [Column("stelle")]
    public int Stelle { get; set; }

    [Column("telefono")]
    public string Telefono { get; set; }

    [Column("fk_servizio")]
    public int ServizioId { get; set; }
    public Servizio Servizio { get; set; }
}