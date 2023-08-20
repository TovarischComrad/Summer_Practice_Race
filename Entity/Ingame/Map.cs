using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class Map : BaseObject
    {
        public Map(string name) : base() {
            ObjectType = "Map";
            Width = 4000;
            Height = 6000;
            Left = -250;
            Bottom = -2000;
            //BackgroundImage = "url(/texture/map/)" + name + ")";
            BackgroundImage = "url(/texture/map/indi2.png)";
        }

    }
}
