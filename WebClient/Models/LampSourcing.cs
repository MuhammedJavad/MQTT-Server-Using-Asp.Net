using System;

namespace WebClient.Models
{
    public class LampSourcing
    {
        public LampSourcing()
        {
            Time = DateTime.Now;
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
    }
}