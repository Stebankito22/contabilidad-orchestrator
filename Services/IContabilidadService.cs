using ContabilidadOrchestrator.Models;
using System.Text.Json;

public interface IContabilidadService
{
    Task<DeudaCliente> ObtenerDeudaClienteAsync(string clienteId);
    Task<List<ClienteResumen>> ObtenerResumenClientesAsync();
    Task<List<Factura>> ObtenerFacturasVencidasAsync();
    Task<ApiResponse<string>> SimularPagoFacturaAsync(string facturaId, decimal monto);
}

public class ContabilidadService : IContabilidadService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContabilidadService> _logger;

    public ContabilidadService(HttpClient httpClient, ILogger<ContabilidadService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<DeudaCliente> ObtenerDeudaClienteAsync(string clienteId)
    {
        try
        {
            _logger.LogInformation($"Obteniendo datos combinados para cliente: {clienteId}");

            // ✅ CONSUMIR DOS SERVICIOS REMOTOS con timeout
            var facturasTask = ObtenerFacturasAsync();
            var pagosTask = ObtenerPagosAsync();

            await Task.WhenAll(facturasTask, pagosTask);

            var facturas = await facturasTask;
            var pagos = await pagosTask;

            // ✅ VALIDAR DATOS Y MANEJAR ERRORES PARCIALES
            if (facturas == null && pagos == null)
            {
                throw new Exception("Ambos servicios no responden");
            }

            if (facturas == null)
            {
                _logger.LogWarning("Servicio de Facturas no disponible, usando datos parciales");
                facturas = new List<Factura>();
            }

            if (pagos == null)
            {
                _logger.LogWarning("Servicio de Pagos no disponible, usando datos parciales");
                pagos = new List<Pago>();
            }

            // ✅ REGLAS DE NEGOCIO: Calcular deuda real
            return CalcularDeudaCliente(clienteId, facturas, pagos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener deuda para cliente {clienteId}");
            throw;
        }
    }

    // ... (los otros métodos permanecen iguales, pero agregamos mejor manejo de errores)

    private async Task<List<Factura>> ObtenerFacturasAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));
            var response = await _httpClient.GetAsync(
                "https://programacionweb2examen3-production.up.railway.app/api/Facturas/Listar",
                cts.Token);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var facturas = JsonSerializer.Deserialize<List<Factura>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // ✅ NULL CHECKS
                return facturas?.Where(f => f != null && !f.Borrado).ToList() ?? new List<Factura>();
            }

            _logger.LogWarning($"Servicio de Facturas respondió con: {response.StatusCode}");
            return null;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Timeout en servicio de Facturas");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en servicio de Facturas");
            return null;
        }
    }

    private async Task<List<Pago>> ObtenerPagosAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(25));
            var response = await _httpClient.GetAsync(
                "https://programacionweb2examen3-production.up.railway.app/api/Pagos/Listar",
                cts.Token);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var pagos = JsonSerializer.Deserialize<List<Pago>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // ✅ NULL CHECKS
                return pagos?.Where(p => p != null && !p.Borrado).ToList() ?? new List<Pago>();
            }

            _logger.LogWarning($"Servicio de Pagos respondió con: {response.StatusCode}");
            return null;
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Timeout en servicio de Pagos");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en servicio de Pagos");
            return null;
        }
    }

    // ✅ REGLAS DE NEGOCIO IMPLEMENTADAS
    private DeudaCliente CalcularDeudaCliente(string clienteId, List<Factura> facturas, List<Pago> pagos)
    {
        var facturasCliente = facturas.Where(f => f.ClienteId == clienteId).ToList();
        var pagosCliente = pagos.Where(p => p.ClienteId == clienteId).ToList();

        var facturasDetalle = new List<FacturaDetalle>();
        var totalFacturado = facturasCliente.Sum(f => f.Monto);
        var totalPagado = pagosCliente.Sum(p => p.Monto);
        var deudaPendiente = totalFacturado - totalPagado;

        foreach (var factura in facturasCliente)
        {
            var montoPagado = pagosCliente.Where(p => p.FacturaId == factura.FacturaId).Sum(p => p.Monto);
            var saldoPendiente = factura.Monto - montoPagado;

            // ✅ REGLA: Determinar si está vencida (más de 30 días)
            var estaVencida = (DateTime.Now - factura.Fecha).TotalDays > 30 && saldoPendiente > 0;

            facturasDetalle.Add(new FacturaDetalle
            {
                FacturaId = factura.FacturaId,
                MontoFactura = factura.Monto,
                MontoPagado = montoPagado,
                SaldoPendiente = saldoPendiente,
                FechaEmision = factura.Fecha,
                Estado = saldoPendiente == 0 ? "PAGADA" : (estaVencida ? "VENCIDA" : "PENDIENTE"),
                EstaVencida = estaVencida
            });
        }

        // ✅ REGLA: Estado de crédito del cliente
        var estadoCredito = deudaPendiente == 0 ? "AL DÍA" :
                           facturasDetalle.Any(f => f.EstaVencida) ? "MOROSO" :
                           "PENDIENTE";

        return new DeudaCliente
        {
            ClienteId = clienteId,
            TotalFacturado = totalFacturado,
            TotalPagado = totalPagado,
            DeudaPendiente = deudaPendiente,
            Facturas = facturasDetalle,
            UltimaActualizacion = DateTime.Now,
            EstadoCredito = estadoCredito,
            FacturasPendientes = facturasDetalle.Count(f => f.SaldoPendiente > 0),
            FacturasPagadas = facturasDetalle.Count(f => f.SaldoPendiente == 0)
        };
    }

    public Task<List<ClienteResumen>> ObtenerResumenClientesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Factura>> ObtenerFacturasVencidasAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<string>> SimularPagoFacturaAsync(string facturaId, decimal monto)
    {
        throw new NotImplementedException();
    }

    // ... (otros métodos)
}