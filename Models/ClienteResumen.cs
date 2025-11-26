namespace ContabilidadOrchestrator.Models
{
    public class ClienteResumen
    {
        public string ClienteId { get; set; }
        public decimal DeudaTotal { get; set; }
        public int TotalFacturas { get; set; }
        public string EstadoCredito { get; set; }
    }
}
