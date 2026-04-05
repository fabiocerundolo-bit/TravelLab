using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelLab.Models;

[Table("t_treni")] 
public class Treno
{
    [Key]
    [Column("id_servizio")]
    public int IdServizio { get; set; }
    public Servizio Servizio { get; set; }
    [Column("numero_treno")]
    public string NumeroTreno { get; set; }
    [Column("tipo_treno")]
    public string TipoTreno { get; set; }
    [Column("fk_mezzo")]
    public int? MezzoId { get; set; }
    public Mezzo Mezzo { get; set; }
}