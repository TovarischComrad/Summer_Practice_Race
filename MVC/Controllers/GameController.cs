using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quartz;
using Quartz.Impl;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;

namespace MVC.Controllers
{
    public class GameController : Controller
    {
        // Начальная страница 
        [Authorize]
        public IActionResult Index()
        {
            var query = new Entity.SearchIn.SearchingGame();
            return View(BL.GameBL.Get(query));
        }

        // Расписание действий для каждой игры
        ConcurrentDictionary<int, IScheduler> gameFlows = new ConcurrentDictionary<int, IScheduler>();
        StdSchedulerFactory factory = new StdSchedulerFactory();

        // Создание игровой сессии
        [Authorize]
        public async Task<IActionResult> Creator()
        {
            // Создание и добавление игры в БД
            var game = new Entity.Game();
            game.Host = BL.UserBL.Get(User.Identity!.Name).Id;
            game.Date = DateTime.Now;
            game.Status = 1;
            game.Id = BL.GameBL.AddOrUpdate(game);

            // Создание и добавление игровой сессии в БД
            var hostSession = new Entity.Session();
            hostSession.User = game.Host;
            hostSession.Game = game.Id;
            hostSession.Id = BL.SessionBL.AddOrUpdate(hostSession);

            // Инициализация игры
            BL.GameLogic.Initialization(game);
            string? color = User.Claims.Where(x => x.Type == "Color").Select(x => x.Value).SingleOrDefault();
            BL.GameLogic.JoinUser(game.Id, game.Host, color);

            // Триггер для бесконечного повторения действий по расписанию
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(game.Id.ToString(), "group1").StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(new TimeSpan(0, 0, 0, 0, 20)).RepeatForever())
                .Build();

            // Описание действия по расписанию
            IJobDetail job = JobBuilder.Create<BL.GamePlay>().UsingJobData("gameId", game.Id).Build();

            // Создание, добавление и запуск расписания для игры game
            gameFlows.TryAdd(game.Id, await factory.GetScheduler());
            await gameFlows[game.Id].Start();
            await gameFlows[game.Id].ScheduleJob(job, trigger);

            return RedirectToAction("Start", "Game", new { gameId = game.Id, userId = game.Host });
        }

        // Запуск игры
        [Authorize]
        public IActionResult Start(int gameId, int userId)
        {
            int[] arr = { gameId, userId };
            return View(arr);
        }

        // Выход из игры
        public IActionResult Return(int gameId, int userId)
        {
            // Модификация записей в БД
            Entity.Game game = BL.GameBL.Get(gameId);
            if (userId == game.Host)
            {
                BL.GameBL.AddOrUpdate(game);
            }
            Entity.Session session = BL.SessionBL.Get(gameId, userId);
            BL.SessionBL.Delete(session);

            // Удаление машины 
            foreach (var id in BL.GameLogic.allGames[gameId].Keys)
            {
                if (BL.GameLogic.allGames[gameId][id].ObjectType == "RaceCar")
                {
                    var car = (Entity.Ingame.RaceCar)BL.GameLogic.allGames[gameId][id];
                    if (car.userId == userId)
                    {
                        BL.GameLogic.allGames[gameId].TryRemove(id, out _); 
                        break;
                    }
                }
            }
            return RedirectToAction("Index", "Game");
        }

        // Подключение к игре
        [Authorize]
        public IActionResult Join(int gameId)
        {
            // Добавление сессии игрока в БД
            var session = new Entity.Session();
            session.User = BL.UserBL.Get(User.Identity!.Name).Id;
            session.Game = gameId;
            session.Id = BL.SessionBL.AddOrUpdate(session);

            // Добавление игровых объектов
            string? color = User.Claims.Where(x => x.Type == "Color").Select(x => x.Value).SingleOrDefault();
            BL.GameLogic.JoinUser(session.Game, session.User, color);

            return RedirectToAction("Start", "Game", new { gameId = session.Game, userId = session.User });
        }

        // Формирование всех игровых объектов
        public IActionResult GetElements(int gameId, int userId)
        {
            if (BL.GameLogic.allGames.ContainsKey(gameId))
            {
                // Не отправлять отдалённые элементы

                Entity.Ingame.BaseObject[] temp = new Entity.Ingame.BaseObject[BL.GameLogic.allGames[gameId].Values.Count];
                BL.GameLogic.allGames[gameId].Values.CopyTo(temp, 0);

                double x = 0;
                double y = 0;
                foreach(var obj in temp)
                {
                    if (obj.ObjectType == "RaceCar")
                    {
                        var car = (Entity.Ingame.RaceCar)obj;
                        if (car.userId == userId)
                        {
                            x = car.Left + car.Width / 2;
                            y = car.Bottom + car.Height / 2;
                            break;
                        }
                    }
                }
                double dx = x - 960;
                double dy = y - 540;

                foreach (var obj in temp)
                {
                    obj.Left -= dx;
                    obj.Bottom -= dy;
                }

                var options = new JsonSerializerOptions
                {
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals
                };
                var json = JsonSerializer.Serialize((object[])temp, options);
                return Ok(json);
            }
            return Ok();
        }

        // Обработчик нажатия клавиш
        public IActionResult PressButton(bool isDown, int gameId, int userId, int btn)
        {
            if (btn == 82 && isDown)
            {
                // Перебор всех игровых объектов
                foreach (var obj in BL.GameLogic.allGames[gameId].Values)
                {
                    if (obj.ObjectType == "RaceCar")
                    {
                        var car = (Entity.Ingame.RaceCar)obj;
                        if (car.userId == userId)
                        {
                            if (car.Front) { car.Front = false; }
                            else { car.Front = true; }
                        }
                    }
                }
                return Ok(Json(true));
            }
            // Добавление клавиши в словарь
            if (isDown)
            {
                if (!BL.GameLogic.allButtons.ContainsKey(gameId))
                {
                    BL.GameLogic.allButtons.TryAdd(gameId, new ConcurrentDictionary<int, ConcurrentDictionary<int, bool>>());
                }
                if (!BL.GameLogic.allButtons[gameId].ContainsKey(userId))
                {
                    BL.GameLogic.allButtons[gameId].TryAdd(userId, new ConcurrentDictionary<int, bool>());
                }
                if (!BL.GameLogic.allButtons[gameId][userId].ContainsKey(btn))
                {
                    BL.GameLogic.allButtons[gameId][userId].TryAdd(btn, true);
                }
            }
            // Удаление клавиши из словаря
            else
            {
                if (BL.GameLogic.allButtons.ContainsKey(gameId) && BL.GameLogic.allButtons[gameId].ContainsKey(userId))
                {
                    if (BL.GameLogic.allButtons[gameId][userId].ContainsKey(btn))
                    {
                        bool success;
                        BL.GameLogic.allButtons[gameId][userId].Remove(btn, out success);
                    }
                }
            }
            return Ok(Json(true));
        }

        public IActionResult Options()
        {
            return View();
        }

        [Authorize]
        async public Task<IActionResult> ChangeColor(string color)
        {

            // Обновление куки файла с помощщщью его пересоздания
            string userName = User.Identity!.Name!;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName),
                new Claim("Color", color)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));

            return RedirectToAction("Options");
        }
    }
}
