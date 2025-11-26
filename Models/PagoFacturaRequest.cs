namespace ContabilidadOrchestrator.Models
{
    public class PagoFacturaRequest
    {
        public string FacturaCodigo { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string Referencia { get; set; }
        public object CodigoFactura { get; internal set; }
        public object MontoPago { get; internal set; }
    }
}
