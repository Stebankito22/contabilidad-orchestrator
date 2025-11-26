namespace ContabilidadOrchestrator.Models
{
    public class Factura
    {
        public string FacturaId { get; set; }
        public string ClienteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public bool Borrado { get; set; }
    }
}
