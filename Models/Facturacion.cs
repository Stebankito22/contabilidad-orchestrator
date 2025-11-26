namespace ContabilidadOrchestrator.Models
{
    public class Facturacion
    {
        public string Id { get; set; }
        public string ClienteId { get; set; }
        public string NumeroFactura { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Estado { get; set; }
        public string Concepto { get; set; }
    }
}
