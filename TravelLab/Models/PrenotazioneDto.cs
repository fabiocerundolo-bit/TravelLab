public class PrenotazioneDto
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteCognome { get; set; }
    public int ViaggioId { get; set; }
    public string Destinazione { get; set; }
    public DateTime DataInizio { get; set; }
    public DateTime DataFine { get; set; }
    public string Stato { get; set; }
    public DateTime DataPrenotazione { get; set; }
}