using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class SessionBL
    {
        static public int AddOrUpdate(Entity.Session session)
        {
            return DAL.SessionDAL.AddOrUpdate(session);
        }

        static public void Delete(Entity.Session session)
        {
            DAL.SessionDAL.Delete(session);
        }

        static public Entity.Session Get(int gameId, int userId)
        {
            return DAL.SessionDAL.Get(gameId, userId);
        }
    }
}
