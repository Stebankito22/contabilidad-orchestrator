using ContabilidadOrchestrator.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class DeudaController : ControllerBase
{
    private readonly IContabilidadService _contabilidadService;
    private readonly ILogger<DeudaController> _logger;

    public DeudaController(IContabilidadService contabilidadService, ILogger<DeudaController> logger)
    {
        _contabilidadService = contabilidadService;
        _logger = logger;
    }

    [HttpGet("cliente/{clienteId}")]
    public async Task<ActionResult<ApiResponse<DeudaCliente>>> ObtenerDeudaCliente(string clienteId)
    {
        try
        {
            if (string.IsNullOrEmpty(clienteId))
            {
                return BadRequest(new ApiResponse<DeudaCliente>
                {
                    Success = false,
                    Message = "El ID del cliente es requerido",
                    Timestamp = DateTime.UtcNow
                });
            }

            var deudaCliente = await _contabilidadService.ObtenerDeudaClienteAsync(clienteId);

            return Ok(new ApiResponse<DeudaCliente>
            {
                Success = true,
                Message = "Deuda del cliente obtenida correctamente",
                Data = deudaCliente,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error al obtener deuda para cliente {clienteId}");

            return StatusCode(500, new ApiResponse<DeudaCliente>
            {
                Success = false,
                Message = $"Error interno del servidor: {ex.Message}",
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("reporte/{clienteId}")]
    public async Task<ActionResult<ApiResponse<DeudaCliente>>> ObtenerReporteDeuda(string clienteId)
    {
        // Endpoint alternativo para reportes
        return await ObtenerDeudaCliente(clienteId);
    }
}