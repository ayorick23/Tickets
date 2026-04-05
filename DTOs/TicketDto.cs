namespace Tickets.DTOs
{
    public class TicketDto
    {
        public string? Id { get; set; }

        public string Title { get; set; }
        

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public decimal Hour { get; set; }

        public string UserId { get; set; }

        public string? Description { get; set; }
    }
}
