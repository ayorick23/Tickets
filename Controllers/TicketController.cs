using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tickets.Application.Services;
using Tickets.Domain.Entities;
using Tickets.DTOs;

namespace Tickets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketController : ControllerBase
    {
        private readonly TicketsCase _ticketsCase;

        public TicketController(TicketsCase ticketsCase)
        {
            _ticketsCase = ticketsCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create(TicketDto ticket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Datos insuficientes" });
            }

            var userId = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Ok(await _ticketsCase.Add(new Ticket()
            {
                CreatedBy = userId,
                CreateAt = DateTime.UtcNow,
                Description = ticket.Description,
                EndDate = ticket.End,
                StartDate = ticket.Start,
                Hours = ticket.Hour,
                Title = ticket.Title,
                UserId = ticket.UserId,
                IsDeleted = false,
                Status = TicketStatus.Open
            }));
        }

        [HttpGet("{ticketId:int}")]
        public async Task<IActionResult> GetById(int ticketId)
        {
            return Ok(await _ticketsCase.FindByIdAsync(ticketId));
        }
    }
}
