using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DigitalAgency.Api.Models;
using DigitalAgency.Bll.DTOs;
using DigitalAgency.Bll.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DigitalAgency.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderServiceOrder;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderController> _logger;
        private readonly IValidator<OrderDTO> _serviceOrderValidator;
        public OrderController(IOrderService orderServiceOrder, IMapper mapper, IValidator<OrderDTO> serviceOrderValidator, ILogger<OrderController> logger)
        {
            _orderServiceOrder = orderServiceOrder;
            _mapper = mapper;
            _serviceOrderValidator = serviceOrderValidator;
            _logger = logger;

        }

        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            _logger.LogInformation("Star logging - method GetService controller ServiceOrderContoller");
            var result = await _orderServiceOrder.GetOrdersAsync();
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }
        [HttpGet("/fullOrder")]
        public async Task<IActionResult> GetFullOrder()
        {
            _logger.LogInformation("Star logging - method GetService controller ServiceOrderContoller");
            var result = await _orderServiceOrder.GetFullOrdersAsync();
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }

        [HttpGet("/BusyTime/{date}")]
        public async Task<IActionResult> GetBusyTimeByDay(DateTime date)
        {
            var result = await _orderServiceOrder.GetBusyTimeByDay(date);
            return Ok(result);
        }

        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder(Order order)
        {
            _logger.LogInformation("Star logging - method ScheduleService controller ServiceOrderContoller");
            await _orderServiceOrder.CreateOrderAsync(order);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(Order order)
        {
            _logger.LogInformation("Star logging - method ScheduleService controller ServiceOrderContoller");
            await _orderServiceOrder.UpdateAsync(order);
            return Ok();
        }

        [HttpPost("scheduleservice")]
        public IActionResult ScheduleOrder([FromBody] ScheaduleViewModel model)
        {
            _logger.LogInformation("Star logging - method ScheduleService controller ServiceOrderContoller");
            var order = _mapper.Map<ScheaduleViewModel, OrderDTO>(model);
            var validate = _serviceOrderValidator.Validate(order);
            if (!validate.IsValid)
            {
                return BadRequest(validate.Errors.Select(x => new { Error = x.ErrorMessage, Code = x.ErrorCode }).ToList());
            }
            var result = _orderServiceOrder.ScheduleOrder(order);
            _logger.LogDebug("Time request {Time}", DateTime.UtcNow);
            return Ok(result);
        }

        [HttpPatch("/ChangeServiceState/{id},{state}")]
        public IActionResult ChangeOrderState(int id, string state)
        {
            if (_orderServiceOrder.ChangeOrderState(id, state))
                return Ok();
            return BadRequest();
        }

        [HttpDelete("/DeleteService/{id}")]
        public IActionResult DeleteOrder(int id)
        {
            if (_orderServiceOrder.DeleteOrder(id))
                return Ok();
            return BadRequest();
        }
    }
}