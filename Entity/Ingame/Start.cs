using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class Start : BaseObject
    {
        public Start() : base()
        {
            ObjectType = "Start";
            Width = 10;
            Height = 10;
            Left = 0;
            Bottom = 0;
        }
    }
}
