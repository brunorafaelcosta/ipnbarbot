using System;

namespace ipnbarbot.Application.Models
{
    public class User
    {
        public int Id { get; set; }
        public string ChannelAccountId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public DateTime JoinDate { get; set; }
    }
}