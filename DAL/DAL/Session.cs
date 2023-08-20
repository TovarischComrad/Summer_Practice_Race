using System;
using System.Collections.Generic;

#nullable disable

namespace DAL
{
    public partial class Session
    {
        public int Id { get; set; }
        public int User { get; set; }
        public int Game { get; set; }

        public virtual Game GameNavigation { get; set; }
        public virtual User UserNavigation { get; set; }
    }
}
