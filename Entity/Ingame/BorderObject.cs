using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class BorderObject : BaseObject
    {
        public BorderObject() : base()
        {
            ObjectType = "Border";
            Width = 25;
            Height = 100;
            Left = 0;
            Bottom = 0;
            BackgroundImage = "url(assets/img/hor_border_small.png)";
        }
    }
}
