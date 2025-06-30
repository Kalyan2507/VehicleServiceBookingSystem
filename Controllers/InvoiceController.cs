using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleServiceBook.Models.DTOS;
using VehicleServiceBook.Services;

namespace VehicleServiceBook.Controllers

{

    [Authorize]

    [Route("api/[controller]")]

    [ApiController]

    public class InvoiceController : ControllerBase

    {

        private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)

        {

            _service = service;

        }

        private string GetEmail() => User.FindFirstValue(ClaimTypes.Email);

        [HttpGet]

        public async Task<IActionResult> GetAll()

        {

            var result = await _service.GetAllAsync(GetEmail());

            return Ok(result);

        }

        [HttpGet("{id}")]

        [Authorize(Roles = "User")]

        public async Task<IActionResult> GetById(int id)

        {

            var result = await _service.GetByIdAsync(id, GetEmail());

            return result == null ? NotFound("Invoice not found or access denied") : Ok(result);

        }

        [HttpPost]

        [Authorize(Roles = "ServiceCenter")]

        public async Task<IActionResult> Create(CreateInvoiceDto dto)

        {

            var email = User.FindFirstValue(ClaimTypes.Email);

            var result = await _service.CreateAsync(dto, email);

            if (result is string message)

            {

                if (message == "Already paid")

                    return BadRequest(new { error = message });

                return BadRequest(new { error = message });

            }

            if (result == null)

                return Forbid("Unauthorized or invalid booking.");

            return Ok(result);

        }

        [HttpGet("center")]

        [Authorize(Roles = "ServiceCenter")]

        public async Task<IActionResult> GetForServiceCenter()

        {

            var result = await _service.GetInvoicesForServiceCenterAsync(GetEmail());

            return Ok(result);

        }

        [HttpPut("{id}/status")]

        [Authorize(Roles = "User")]

        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdatePaymentStatusDto dto)

        {

            var email = GetEmail();

            var result = await _service.UpdatePaymentStatusAsync(id, dto.PaymentStatus, email);

            return result ? Ok("Payment status updated") : NotFound("Invoice not found or not your invoice");

        }

    }

}

