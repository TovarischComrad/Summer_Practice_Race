using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class Game
    {
        public Game()
        {
            Sessions = new HashSet<Session>();
        }

        public int Id { get; set; }
        public int Host { get; set; }
        public int Status { get; set; }
        public DateTime Date { get; set; }

        public virtual User HostNavigation { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
