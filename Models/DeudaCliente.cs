namespace ContabilidadOrchestrator.Models
{
    public class DeudaCliente
    {
        public string ClienteId { get; set; }
        public decimal TotalFacturado { get; set; }
        public decimal TotalPagado { get; set; }
        public decimal DeudaPendiente { get; set; }
        public List<FacturaDetalle> Facturas { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public string EstadoCredito { get; internal set; }
        public int FacturasPagadas { get; internal set; }
        public int FacturasPendientes { get; internal set; }
    }
}
