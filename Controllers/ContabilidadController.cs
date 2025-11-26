using ContabilidadOrchestrator.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ContabilidadController : ControllerBase
{
    private readonly IContabilidadService _service;
    private readonly ILogger<ContabilidadController> _logger;

    public ContabilidadController(IContabilidadService service, ILogger<ContabilidadController> logger)
    {
        _service = service;
        _logger = logger;
    }

    // ✅ ENDPOINT REQUERIDO: /api/combined/{id}
    [HttpGet("combined/{clienteId}")]
    public async Task<ActionResult<ApiResponse<DeudaCliente>>> GetCombined(string clienteId)
    {
        try
        {
            _logger.LogInformation($"Solicitando datos combinados para cliente: {clienteId}");

            if (string.IsNullOrEmpty(clienteId))
            {
                return BadRequest(new ApiResponse<DeudaCliente>
                {
                    Success = false,
                    Message = "El ID del cliente es requerido",
                    Data = null
                });
            }

            var deuda = await _service.ObtenerDeudaClienteAsync(clienteId);

            return Ok(new ApiResponse<DeudaCliente>
            {
                Success = true,
                Message = "Datos combinados obtenidos exitosamente",
                Data = deuda
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error en combined endpoint para cliente {clienteId}");

            return StatusCode(500, new ApiResponse<DeudaCliente>
            {
                Success = false,
                Message = $"Error interno del servidor: {ex.Message}",
                Data = null
            });
        }
    }

    // Endpoints adicionales que ya teníamos
    [HttpGet("deuda/{clienteId}")]
    public async Task<ActionResult<DeudaCliente>> GetDeudaCliente(string clienteId)
    {
        var deuda = await _service.ObtenerDeudaClienteAsync(clienteId);
        return Ok(deuda);
    }

    [HttpGet("clientes")]
    public async Task<ActionResult<List<ClienteResumen>>> GetClientes()
    {
        var clientes = await _service.ObtenerResumenClientesAsync();
        return Ok(clientes);
    }

    [HttpGet("facturas/vencidas")]
    public async Task<ActionResult<List<Factura>>> GetFacturasVencidas()
    {
        var facturas = await _service.ObtenerFacturasVencidasAsync();
        return Ok(facturas);
    }

    [HttpPost("pagos/simular")]
    public async Task<ActionResult<ApiResponse<string>>> SimularPago([FromBody] PagoRequest request)
    {
        var resultado = await _service.SimularPagoFacturaAsync(request.FacturaId, request.Monto);
        return Ok(resultado);
    }
}