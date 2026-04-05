using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TravelLab.Models;


[Table("t_navi")] 
public class Nave
{
    [Key]
    [Column("id_servizio")]
    public int IdServizio { get; set; }
    public Servizio Servizio { get; set; }
    [Column("nome_nave")]
    public string NomeNave { get; set; }
    [Column("fk_mezzo")]
    public int? MezzoId { get; set; }
    public Mezzo Mezzo { get; set; }
}