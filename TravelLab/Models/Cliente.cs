namespace TravelLab.Models;

using System.ComponentModel.DataAnnotations.Schema;


[Table("t_clienti")]
public class Cliente
{
    [Column("id_cliente")]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("cognome")]
    public string Cognome { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("telefono")]
    public string Telefono { get; set; }

    [Column("indirizzo")]
    public string Indirizzo { get; set; }

    public ICollection<Prenotazione> Prenotazioni { get; set; } = new List<Prenotazione>();
} 
