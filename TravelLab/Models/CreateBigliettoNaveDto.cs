namespace TravelLab.Models
{
    public class CreateBigliettoNaveDto
    {
        public int PrenotazioneId { get; set; }
        public int NaveId { get; set; }        // id_servizio della nave
        public decimal PrezzoEffettivo { get; set; }
    }
}