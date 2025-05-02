using Microsoft.AspNetCore.Mvc;
using VerticalLogistica.Domain.Entities;
using VerticalLogistica.Domain.Filters;
using VerticalLogistica.Domain.Interfaces;

namespace VerticalLogistica.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Get orders with optional filtering
        /// </summary>
        /// <param name="orderId">Optional order ID filter</param>
        /// <param name="startDate">Optional start date filter</param>
        /// <param name="endDate">Optional end date filter</param>
        /// <returns>List of users with their orders</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<User>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetOrders(
            [FromQuery] int? orderId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            OrderFilter? filter = null;
            if (orderId.HasValue || startDate.HasValue || endDate.HasValue)
            {
                filter = new OrderFilter
                {
                    OrderId = orderId,
                    StartDate = startDate,
                    EndDate = endDate
                };
            }

            var result = await _orderService.GetOrdersAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Upload order file for processing
        /// </summary>
        /// <param name="file">Order file in the legacy format</param>
        /// <returns>Action result</returns>
        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UploadOrderFile(IFormFile? file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Nenhum arquivo foi enviado.");

                using var stream = file.OpenReadStream();
                await _orderService.ProcessOrderFileAsync(stream);

                return Ok(new { Message = "Arquivo processado com sucesso." });
            }
            catch (ApplicationException e)
            {
                return BadRequest(new { mensagem = e.Message });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { mensagem = $"Falha ao salvar arquivo: {e.Message}" });
            }

        }
    }
}
