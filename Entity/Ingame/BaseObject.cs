using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class BaseObject
    {
        public string Id { get; set; }
        public double Left { get; set; }
        public double Bottom { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public string ObjectType { get; set; }
        public string BackgroundImage { get; set; }

        private static int CurrentId = 0;

        public BaseObject()
        {
            CurrentId++;
            Id = CurrentId.ToString();
        }
    }
}
