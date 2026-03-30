using System.ComponentModel.DataAnnotations;

namespace TravelLab.Models;
using System.ComponentModel.DataAnnotations.Schema;

[Table("t_voli")]
public class Volo
{
    [Key]
    [Column("id_servizio")]
    public int ServizioId { get; set; }
    public Servizio Servizio { get; set; }

    [Column("numero_volo")]
    public string NumeroVolo { get; set; }

    [Column("compagnia_aerea")]
    public string CompagniaAerea { get; set; }

    [Column("gate")]
    public string Gate { get; set; }

    [Column("fk_mezzo")]
    public int? MezzoId { get; set; }
}