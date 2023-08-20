using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    static public class UserBL
    {
        static public int AddOrUpdate(Entity.User user)
        {
            return DAL.UserDAL.AddOrUpdate(user);
        }

        static public Entity.User Get(string login)
        {
            return DAL.UserDAL.Get(login);
        }

        public static bool Authorization(Entity.User user)
        {
            var temp = Get(user.Name);
            return temp != null && user.Password == temp.Password;
        }
    }
}
