using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class User
    {
        public User()
        {
            Games = new HashSet<Game>();
            Sessions = new HashSet<Session>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Game> Games { get; set; }
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
