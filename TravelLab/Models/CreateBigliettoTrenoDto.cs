namespace TravelLab.Models
{
    public class CreateBigliettoTrenoDto
    {
        public int PrenotazioneId { get; set; }
        public int TrenoId { get; set; }      // id_servizio del treno
        public decimal PrezzoEffettivo { get; set; }
    }
}