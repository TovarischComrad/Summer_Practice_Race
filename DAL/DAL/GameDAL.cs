using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DAL
{
    public class GameDAL
    {
        // Добавление игры в БД
        static public int AddOrUpdate(Entity.Game game)
        {
            using (RaceContext data = new RaceContext())
            {
                
                var databaseGame = data.Games.FirstOrDefault(x => x.Host == game.Host && x.Status == 1);

                // Если у пользователя нет активной игры
                if (databaseGame == null)
                {
                    databaseGame = new Game();
                    data.Games.Add(databaseGame);
                    var maxId = data.Games.Count() == 0 ? 0 : data.Games.Max(x => x.Id);
                    databaseGame.Id = maxId + 1;
                    databaseGame.Date = game.Date;
                    databaseGame.Host = game.Host;
                    databaseGame.Status = game.Status;
                }
                else
                {
                    databaseGame.Id = game.Id;
                    databaseGame.Date = game.Date;
                    databaseGame.Host = game.Host;
                    databaseGame.Status = 0;
                }
                data.SaveChanges();
                return databaseGame.Id;
            }
        }

        // Нахождение игры в БД по id
        static public Entity.Game Get(int id)
        {
            using (RaceContext data = new RaceContext())
            {
                var databaseGame = data.Games.FirstOrDefault(x => x.Id == id);
                var game = new Entity.Game();
                game.Id = id;
                game.Date = databaseGame.Date;
                game.Host = databaseGame.Host;
                game.Status = databaseGame.Status;
                // game.HostName = databaseGame.HostNavigation.Name;
                // game.PlayersCount = databaseGame.Sessions.Count();
                return game;
            }
        }

        // Получение списка игр из БД
        static public Entity.SearchOut.SearchOutGame Get(Entity.SearchIn.SearchingGame query)
        {
            var result = new Entity.SearchOut.SearchOutGame();  
            using (RaceContext data = new RaceContext())
            {
                // Отбор игр по хосту
                var temp = data.Games.Where(x => !query.Host.HasValue || x.Host.Equals(query.Host));
                temp = temp.Where(x => x.Status == 1);
                result.Total = temp.Count();

                // Показать первые Top игр
                if (query.Top.HasValue)
                {
                    temp = temp.Take(query.Top.Value);
                }

                // Пропустить первые Skip игр
                if (query.Skip.HasValue)
                {
                    temp = temp.Skip(query.Skip.Value);
                }

                // Преобразование результата в список игр
                result.Games = temp.Select(x => new Entity.Game()
                {
                    Id = x.Id,
                    Date = x.Date,
                    Host = x.Host,
                    Status = x.Status,
                    HostName = x.HostNavigation.Name,
                    PlayersCount = x.Sessions.Count()
                }).ToList();
            }
            return result;
        }
    }
}
