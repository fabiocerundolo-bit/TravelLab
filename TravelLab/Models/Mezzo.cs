using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLab.Models
{
    [Table("t_mezzi")]
    public class Mezzo
    {
        [Column("id_mezzo")]
        public int Id { get; set; }

        [Column("tipo_mezzo")]
        public string TipoMezzo { get; set; }

        [Column("compagnia")]
        public string Compagnia { get; set; }

        [Column("capacita")]
        public int Capacita { get; set; }
    }
}