using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelLab.Models
{
    [Table("t_angenzia")]
    public class Agenzia
    {
        [Key]
        [Column("id_agenzia")]
        public int Id { get; set; }

        [Column("nome")]
        public string Nome { get; set; }

        [Column("email")]
        public string Email { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; }

        [Column("indirizzo")]
        public string Indirizzo { get; set; }
    }
}