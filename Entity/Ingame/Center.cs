using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class Center : BaseObject
    {
        public Center() : base()
        {
            ObjectType = "Center";
            Width = 25;
            Height = 25;
            Left = 0;
            Bottom = 0;
        }
    }
}
