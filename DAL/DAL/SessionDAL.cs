using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DAL
{
    public class SessionDAL
    {
        static public int AddOrUpdate(Entity.Session session)
        {
            using (RaceContext data = new RaceContext())
            {
                var databaseSession = data.Sessions.FirstOrDefault(x => x.Game == session.Game && x.User == session.User);
                if (databaseSession == null)
                {
                    databaseSession = new Session();
                    data.Sessions.Add(databaseSession);
                    var maxId = data.Sessions.Count() == 0 ? 0 : data.Sessions.Max(x => x.Id);
                    databaseSession.Id = maxId + 1;
                    databaseSession.Game = session.Game;
                    databaseSession.User = session.User;
                }
                data.SaveChanges();
                return databaseSession.Id;
            }
        }

        static public void Delete(Entity.Session session)
        {
            using (RaceContext data = new RaceContext())
            {
                var databaseSession = data.Sessions.FirstOrDefault(x => x.Game == session.Game && x.User == session.User);
                if (databaseSession != null)
                {
                    data.Sessions.Remove(databaseSession);
                }
                data.SaveChanges();
            }
        }

        static public Entity.Session Get(int gameId, int userId)
        {
            using (RaceContext data = new RaceContext())
            {
                var databaseSession = data.Sessions.FirstOrDefault(x => x.Game == gameId && x.User == userId);
                Entity.Session session = new Entity.Session();
                if (databaseSession != null)
                {
                    session.Id = databaseSession.Id;
                    session.User = databaseSession.User;
                    session.Game = databaseSession.Game;
                }
                return session;
            }
        }
    }
}
