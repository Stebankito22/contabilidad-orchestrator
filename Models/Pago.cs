namespace ContabilidadOrchestrator.Models
{
    public class Pago
    {
        public string PagoId { get; set; }
        public string FacturaId { get; set; }
        public string ClienteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public bool Borrado { get; set; }
    }
}
