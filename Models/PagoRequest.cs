namespace ContabilidadOrchestrator.Models
{
    public class PagoRequest
    {
        public string FacturaId { get; set; }
        public decimal Monto { get; set; }
    }
}
