namespace ContabilidadOrchestrator.Models
{
    public class FacturaDetalle
    {
        public string FacturaId { get; set; }
        public decimal MontoFactura { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
        public DateTime FechaEmision { get; set; }
        public string Estado { get; set; }
        public bool EstaVencida { get; internal set; }
    }
}
