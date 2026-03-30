using System.ComponentModel.DataAnnotations;

namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_servizi")]
public class Servizio
{
    [Key]
    [Column("id_servizio")]
    public int Id { get; set; }

    [Column("tipo_servizio")]
    public string TipoServizio { get; set; }

    [Column("prezzo_base")]
    public decimal PrezzoBase { get; set; }

    public Volo Volo { get; set; }
    public Hotel Hotel { get; set; }
    // Aggiungi Treno/Navi se presenti
}