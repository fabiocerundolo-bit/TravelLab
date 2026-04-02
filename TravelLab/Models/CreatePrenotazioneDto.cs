namespace TravelLab.Models;

public class CreatePrenotazioneDto
{
    public int ClienteId { get; set; }
    public int ViaggioId { get; set; }
    public int AgenziaId { get; set; }
    public DateTime DataPrenotazione { get; set; }
    public string Stato { get; set; }
}