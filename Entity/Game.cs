using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class Game
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Host { get; set; }
        public int Status { get; set; }
        public string HostName { get; set; }
        public int PlayersCount { get; set; }
    }
}
