using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Ingame
{
    public class RaceCar : BaseObject
    {
        // Id водителя
        public int userId { get; set; }
        // Цвет машины
        public string Color { get; set; }

        // Текущая скорость
        public double Speed { get; set; }
        // Боковая скорость
        public double SideSpeed { get; set; }
        // Максиммальная скороть
        public double MaxSpeed { get; set; }

        // Поворот машины в градусах
        public double Rotation { get; set; }
        // Задняя передача
        public bool Front { get; set; }
        // Текущее коэффициент ускорения (для камеры)
        public double Acceleration { get; set; }

        public RaceCar(string color = "pink") : base()
        {
            ObjectType = "RaceCar";
            Width = 40;
            Height = 80;
            Left = 0;
            Bottom = 0;
            Color = color;

            Speed = 0;
            SideSpeed = 0;
            MaxSpeed = 60;

            Rotation = 0;
            Front = true;
            Acceleration = 1;
        }
    }
}
