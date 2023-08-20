using System;
using System.Collections.Generic;
using System.Text;
using Quartz;
using System.Net;
using System.Linq;
using System.Threading.Tasks;


namespace BL
{
    public class GamePlay : IJob
    {
        // Ускорение свободного падения
        public static double g = 9.8154;

        // Координаты центра экрана
        public static double X = 960;
        public static double Y = 540;

        // Метод Execute вызывается c помощью триггера по расписанию
        public Task Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            int gameId = dataMap.GetInt("gameId");

            // Извлечение всех машин
            List<Entity.Ingame.RaceCar> cars = new List<Entity.Ingame.RaceCar>();
            foreach (var obj in BL.GameLogic.allGames[gameId].Values)
            {
                if (obj.ObjectType == "RaceCar")
                {
                    var car = (Entity.Ingame.RaceCar)obj;
                    cars.Add(car);
                    // k = car.Acceleration;
                    BL.GameLogic.cameraDistance[gameId] = car.Acceleration;
                }
            }

            // Просмотр действий для всех пользователей
            foreach (var userId in BL.GameLogic.usersInGame[gameId].Keys)
            {
                // Перебор всех игровых объектов
                foreach (var car in cars)
                {
                    if (car.userId == userId)
                    {
                        // Налево
                        if (BL.GameLogic.allButtons[gameId][userId].ContainsKey(65)
                            || BL.GameLogic.allButtons[gameId][userId].ContainsKey(37))
                        {
                            if (car.Speed > 0) { car.Rotation -= 3; }
                            if (car.Speed < 0) { car.Rotation += 1; }
                        }
                        // Направо
                        if (BL.GameLogic.allButtons[gameId][userId].ContainsKey(68)
                            || BL.GameLogic.allButtons[gameId][userId].ContainsKey(39))
                        {
                            if (car.Speed > 0) { car.Rotation += 3; }
                            if (car.Speed < 0) { car.Rotation -= 1; }
                        }
                        // Вверх
                        if (BL.GameLogic.allButtons[gameId][userId].ContainsKey(87)
                            || BL.GameLogic.allButtons[gameId][userId].ContainsKey(38))
                        {
                            if (car.Front) { car.Speed = Math.Min(car.Speed + 0.25 * car.Acceleration, car.MaxSpeed); }
                            else { car.Speed = Math.Max(car.Speed - 0.2 * car.Acceleration, -car.MaxSpeed / 3.0); }

                        }
                        // Вниз
                        if (BL.GameLogic.allButtons[gameId][userId].ContainsKey(83)
                            || BL.GameLogic.allButtons[gameId][userId].ContainsKey(40))
                        {
                            if (car.Front) { car.Speed = Math.Max(0, car.Speed - 0.25 * car.Acceleration); }
                            else { car.Speed = Math.Min(0, car.Speed + 0.2 * car.Acceleration); }
                        }
                    }

                    // Инерция
                    double nu = 0.005;
                    double a = g * nu;
                    if (car.Speed > 0) { car.Speed = Math.Max(car.Speed - a * car.Acceleration, 0); }
                    if (car.Speed < 0) { car.Speed = Math.Min(car.Speed + a * car.Acceleration, 0); }
                    if (car.SideSpeed > 0) { car.SideSpeed = Math.Max(car.SideSpeed - a * car.Acceleration, 0); }
                    if (car.SideSpeed < 0) { car.SideSpeed = Math.Min(car.SideSpeed + a * car.Acceleration, 0); }

                    // Удары машин
                    bool f = false;
                    var d = 4;
                    double rad1 = car.Rotation / 180.0 * Math.PI;

                    var ax = car.Left + car.Speed * Math.Sin(rad1) - d;
                    var ax1 = car.Left + car.Width + car.Speed * Math.Cos(rad1) + d;
                    var ay = car.Bottom + car.Speed * Math.Sin(rad1) - d;
                    var ay1 = car.Bottom + car.Height + car.Speed * Math.Cos(rad1) + d;

                    var crash = new Entity.Ingame.RaceCar();

                    foreach (var car2 in cars)
                    {
                        var bx = car2.Left - d;
                        var bx1 = car2.Left + car2.Width + d;
                        var by = car2.Bottom - d;
                        var by1 = car2.Bottom + car2.Height + d;

                        if (car.Id != car2.Id && BL.GameLogic.Intersects(ax, ax1, ay, ay1, bx, bx1, by, by1, true, car.Rotation, true, car2.Rotation))
                        {
                            f = true;
                            crash = car2;
                            break;
                        }
                    }

                    double w1 = Math.Sqrt(car.Speed * car.Speed + car.SideSpeed * car.SideSpeed) * Math.Sign(car.Speed);
                    double gamma1 = 0;
                    if (car.Speed != 0)
                    {
                        gamma1 = Math.Atan(car.SideSpeed / car.Speed);
                    }

                    // Если произошло столкновение
                    if (f)
                    {
                        car.Bottom -= w1 * Math.Cos(rad1);
                        car.Left -= w1 * Math.Sin(rad1);

                        //double w2 = Math.Sqrt(crash.Speed * crash.Speed + crash.SideSpeed * crash.SideSpeed) * Math.Sign(crash.Speed);
                        //double gamma2 = 0;
                        //if (crash.Speed != 0)
                        //{
                        //    gamma2 = Math.Atan(crash.SideSpeed / crash.Speed);
                        //}
                        //double rad2 = crash.Rotation / 180.0 * Math.PI;

                        //double x1 = w1 * Math.Sin(gamma1 + rad1);
                        //double y1 = w1 * Math.Cos(gamma1 + rad1);
                        //double x2 = w2 * Math.Sin(gamma2 + rad2);
                        //double y2 = w2 * Math.Cos(gamma2 + rad2);

                        //double _X = x1 + x2;
                        //double _Y = y1 + y2;

                        //double W = Math.Sqrt(_X * _X + _Y * _Y);
                        //double theta = 0;
                        //if (_Y != 0)
                        //{
                        //    theta = Math.Atan(_X / _Y);
                        //}

                        //car.SideSpeed = W * Math.Sin(theta - rad1);
                        //car.Speed = W * Math.Cos(theta - rad1);

                        //crash.SideSpeed = W * Math.Sin(theta - rad2);
                        //crash.Speed = W * Math.Cos(theta - rad2);

                        car.Speed *= -1;
                    }
                    // Изменение положения машины при движении
                    else
                    {
                        if (car.SideSpeed > 0) { car.SideSpeed = Math.Min(car.SideSpeed, 20); }
                        if (car.SideSpeed < 0) { car.SideSpeed = Math.Max(car.SideSpeed, -20); }
                        if (car.Speed > 0) { car.Speed = Math.Min(car.Speed, car.MaxSpeed); }
                        if (car.Speed < 0) { car.Speed = Math.Max(car.Speed, -car.MaxSpeed); }

                        car.Bottom += w1 * Math.Cos(rad1 + gamma1);
                        car.Left += w1 * Math.Sin(rad1 + gamma1);
                    } 
                }
            }

            /////////////////////////////////////////////////
            ///    Расчёт положения центра                ///
            /////////////////////////////////////////////////

            //// Нахождение центра всех игроков
            //double x = 0;
            //double y = 0;
            //int cnt = 0;
            //foreach (var car in cars)
            //{
            //    x += car.Left + car.Width / 2; ;
            //    y += car.Bottom + car.Height / 2;
            //    cnt++;
            //}
            //x /= cnt;
            //y /= cnt;

            //// Сдвиг центра
            //foreach (var obj in BL.GameLogic.allGames[gameId].Values)
            //{
            //    if (obj.ObjectType == "Center")
            //    {
            //        obj.Left = x;
            //        obj.Bottom = y;
            //        x -= X;
            //        y -= Y;
            //        break;
            //    }
            //}

            //// Сдвиг всех игровых объектов 
            //foreach (var obj in BL.GameLogic.allGames[gameId].Values)
            //{
            //    obj.Left -= x;
            //    obj.Bottom -= y;
            //}

            //if (cars.Count == 1)
            //{
            //    return Task.CompletedTask;
            //}

            /////////////////////////////////////////////////
            ///    Расчёт положения камеры                ///
            /////////////////////////////////////////////////

            //// Нахождение максимального расстояния между машинами
            //double max_dist = double.MinValue;
            //bool axis = true;   // x - true, y - false
            //double dx = 0;
            //double dy = 0;
            //foreach (var car1 in cars)
            //{
            //    double x1 = car1.Left + car1.Width / 2;
            //    double y1 = car1.Bottom + car1.Height / 2;
            //    foreach (var car2 in cars)
            //    {
            //        double x2 = car2.Left + car2.Width / 2;
            //        double y2 = car2.Bottom + car2.Height / 2;

            //        dx = Math.Abs(x2 - x1);
            //        dy = Math.Abs(y2 - y1);

            //        double dist = Math.Max(dx, dy);

            //        if (dist > max_dist)
            //        {
            //            max_dist = dist;
            //            if (dx >= dy) { axis = true; }
            //            else { axis = false; }
            //        }
            //    }
            //}

            /////////////////////////////////////////////////
            ///    Отдаление камеры                       ///
            /////////////////////////////////////////////////

            //bool fl = false;
            //double b1 = 1100;
            //double b2 = 550;
            //// Если машины вышли за допустимые границы
            //if (axis && max_dist > b1)
            //{
            //    dx = max_dist - b1;
            //    dy = dx * Y / X;
            //    fl = true;
            //}
            //if (!axis && max_dist > b2)
            //{
            //    dy = max_dist - b2;
            //    dx = dy * X / Y;
            //    fl = true;
            //}
            //if (fl)
            //{
            //    double Wx = 2 * X;
            //    double Wy = 2 * Y;
            //    double Vx = Wx - 2 * dx;
            //    double Vy = Wy - 2 * dy;
            //    double kx = Vx / Wx;
            //    double ky = Vy / Wy;

            //    foreach (var obj in BL.GameLogic.allGames[gameId].Values)
            //    {
            //        obj.Left = kx * obj.Left + dx;
            //        obj.Bottom = ky * obj.Bottom + dy;
            //        obj.Width *= kx;
            //        obj.Height *= ky;
            //        if (obj.ObjectType == "RaceCar")
            //        {
            //            Entity.Ingame.RaceCar car = (Entity.Ingame.RaceCar)obj;
            //            car.Acceleration *= kx;
            //        }
            //    }
            //    return Task.CompletedTask;
            //}

            /////////////////////////////////////////////////
            ///    Приближение камеры                     ///
            /////////////////////////////////////////////////

            //double b3 = 200 / BL.GameLogic.cameraDistance[gameId];
            //double b4 = 100 / BL.GameLogic.cameraDistance[gameId];
            //fl = false;
            //if (axis && max_dist > 100 && max_dist < b3)
            //{
            //    dx = b3 - max_dist;
            //    dy = dx * Y / X;
            //    fl = true;
            //}
            //if (!axis && max_dist > 100 && max_dist < b4)
            //{
            //    dy = b4 - max_dist;
            //    dx = dy * X / Y;
            //    fl = true;
            //}
            //if (fl)
            //{
            //    double Wx = 2 * X;
            //    double Wy = 2 * Y;
            //    double Vx = Wx + 2 * dx;
            //    double Vy = Wy + 2 * dy;
            //    double kx = Vx / Wx;
            //    double ky = Vy / Wy;

            //    foreach (var obj in BL.GameLogic.allGames[gameId].Values)
            //    {
            //        obj.Left = kx * obj.Left + dx;
            //        obj.Bottom = ky * obj.Bottom + dy;
            //        obj.Width *= kx;
            //        obj.Height *= ky;
            //        if (obj.ObjectType == "RaceCar")
            //        {
            //            Entity.Ingame.RaceCar car = (Entity.Ingame.RaceCar)obj;
            //            car.Acceleration *= kx;
            //        }
            //    }
            //}

            return Task.CompletedTask;  
        }

    }
}
