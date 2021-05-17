using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalAgency.Bll.Models;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;
        public OrderController(
            IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<OrderModel>>> GetOrder()
        {
            _logger.LogInformation("Star logging - method GetService controller ServiceOrderContoller");
            var result = await _orderService.GetOrdersAsync();
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }

        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(OrderModel order)
        {
            _logger.LogInformation("Star logging - method ScheduleService controller ServiceOrderContoller");
            await _orderService.CreateOrderAsync(order);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(OrderModel order)
        {
            _logger.LogInformation("Star logging - method ScheduleService controller ServiceOrderContoller");
            await _orderService.UpdateAsync(order);
            return Ok();
        }
    }
}