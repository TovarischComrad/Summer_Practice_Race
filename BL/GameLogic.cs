using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using BL.Geometry;

namespace BL
{
    public static class GameLogic
    {
        // Информация обо всех текущих игр
        public static ConcurrentDictionary<int, ConcurrentDictionary<string, Entity.Ingame.BaseObject>> allGames 
            = new ConcurrentDictionary<int, ConcurrentDictionary<string, Entity.Ingame.BaseObject>>();

        // Информация об игроках
        public static ConcurrentDictionary<int, ConcurrentDictionary<int, bool>> usersInGame 
            = new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>();

        // Информация о нажатых клавишах
        public static ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>> allButtons
            = new ConcurrentDictionary<int, ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>>();

        // Информация о масштабе отображения мира в каждой игре
        public static ConcurrentDictionary<int, double> cameraDistance =
            new ConcurrentDictionary<int, double>();

        public static void Initialization(Entity.Game game)
        {
            allButtons.TryAdd(game.Id, new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>());
            usersInGame.TryAdd(game.Id, new ConcurrentDictionary<int, bool>());
            cameraDistance.TryAdd(game.Id, 1.0);

            var arr = new ConcurrentDictionary<string, Entity.Ingame.BaseObject>();

            // Трасса
            var map = new Entity.Ingame.Map("indi2.png");
            arr.TryAdd(map.Id, map);

            // Объект для расчёта положения камеры
            var center = new Entity.Ingame.Center();
            arr.TryAdd(center.Id, center);

            // Объект для расчёта стартовой позиции игрока
            var start = new Entity.Ingame.Start();
            start.Left = 1150 + map.Left;
            start.Bottom = 2450 + map.Bottom;
            arr.TryAdd(start.Id, start);

            allGames.TryAdd(game.Id, arr);
        }

        public static void JoinUser(int gameId, int userId, string color = "pink")
        {
            allButtons[gameId].TryAdd(userId, new ConcurrentDictionary<int, bool>());
            usersInGame[gameId].TryAdd(userId, true);

            // Поиск стартового расположения
            Entity.Ingame.Start start = new Entity.Ingame.Start();
            foreach (var obj in allGames[gameId].Values)
            {
                if (obj.ObjectType == "Start")
                {
                    start = (Entity.Ingame.Start)obj;
                    break;
                }
            }

            // Инициализация машины
            var pl = new Entity.Ingame.RaceCar(color);
            pl.Left = start.Left;
            pl.Bottom = start.Bottom;
            pl.Width *= cameraDistance[gameId];
            pl.Height *= cameraDistance[gameId];
            pl.Acceleration = cameraDistance[gameId];
            pl.userId = userId;
            allGames[gameId].TryAdd(pl.Id, pl);

            // Сдвиг стартового расположения
            start.Left += 50;
        }

        public static bool Intersects(double ax, double ax1, double ay, double ay1,
                                      double bx, double bx1, double by, double by1,
                                      bool car1 = false, double angle1 = 0,
                                      bool car2 = false, double angle2 = 0)
        {
            

            Point[] A = new Point[5];
            Point[] B = new Point[5];

            A[0] = new Point(ax, ay);
            A[1] = new Point(ax, ay1);
            A[2] = new Point(ax1, ay1);
            A[3] = new Point(ax1, ay);
            A[4] = new Point(ax, ay);

            if (car1)
            {
                var rad = -angle1 / 180.0 * Math.PI;
                var cx = (ax + ax1) / 2;
                var cy = (ay + ay1) / 2;
                for (int i = 0; i < 5; i++)
                {
                    A[i].Translate(-cx, -cy);
                    A[i].Rotate(rad);
                    A[i].Translate(cx, cy);
                }
            }

            B[0] = new Point(bx, by);
            B[1] = new Point(bx, by1);
            B[2] = new Point(bx1, by1);
            B[3] = new Point(bx1, by);
            B[4] = new Point(bx, by);

            if (car2)
            {
                var rad = -angle2 / 180.0 * Math.PI;
                var cx = (bx + bx1) / 2;
                var cy = (by + by1) / 2;
                for (int i = 0; i < 5; i++)
                {
                    B[i].Translate(-cx, -cy);
                    B[i].Rotate(rad);
                    B[i].Translate(cx, cy);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (Geometry.Functions.Intersect(A[i], A[i + 1], B[j], B[j + 1]))
                    {
                        return true;
                    }
                }
            }

            return false;
            

            //if (o1.ObjectType == "RaceCar")
            //{
            //    var cx = (ax + ax1) / 2;
            //    var cy = (ay + ay1) / 2;
            //}

            

            //return (
            //    (
            //        (
            //            (ax >= bx && ax <= bx1) || (ax1 >= bx && ax1 <= bx1)
            //        ) && (
            //            (ay >= by && ay <= by1) || (ay1 >= by && ay1 <= by1)
            //        )
            //    ) || (
            //        (
            //            (bx >= ax && bx <= ax1) || (bx1 >= ax && bx1 <= ax1)
            //        ) && (
            //            (by >= ay && by <= ay1) || (by1 >= ay && by1 <= ay1)
            //        )
            //    )
            //) || (
            //    (
            //        (
            //            (ax >= bx && ax <= bx1) || (ax1 >= bx && ax1 <= bx1)
            //        ) && (
            //            (by >= ay && by <= ay1) || (by1 >= ay && by1 <= ay1)
            //        )
            //    ) || (
            //        (
            //            (bx >= ax && bx <= ax1) || (bx1 >= ax && bx1 <= ax1)
            //        ) && (
            //            (ay >= by && ay <= by1) || (ay1 >= by && ay1 <= by1)
            //        )
            //    )
            //);
        }
    }
}
