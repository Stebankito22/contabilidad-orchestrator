namespace ContabilidadOrchestrator.Models
{
    public class ReporteContable
    {
        public DateTime FechaGeneracion { get; set; }
        public decimal TotalFacturadoPeriodo { get; set; }
        public decimal TotalCobradoPeriodo { get; set; }
        public decimal DeudaTotalClientes { get; set; }
        public int ClientesConDeuda { get; set; }
        public int ClientesAlDia { get; set; }
        public List<ClienteResumen> TopClientesDeudores { get; set; }
        public List<FacturaDetalle> FacturasVencidas { get; set; }
    }
}
