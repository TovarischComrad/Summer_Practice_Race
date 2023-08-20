using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DAL
{
    public class UserDAL
    {
        // Добавление нового пользователя в БД
        static public int AddOrUpdate(Entity.User user)
        {
            using (RaceContext data = new RaceContext())
            {
                // Пользователь ищется по логину
                var databaseUser = data.Users.FirstOrDefault(x => x.Name == user.Name);
                // Если пользователь не найден, то добавить его в БД
                if (databaseUser == null)
                {
                    databaseUser = new User();
                    data.Users.Add(databaseUser);

                    databaseUser.Name = user.Name;
                    databaseUser.Password = user.Password;

                    var maxId = data.Users.Count() == 0 ? 0 : data.Users.Max(x => x.Id);

                    databaseUser.Id = maxId + 1;
                }
                // Иначе ошибка, пользователь уже существует 

                data.SaveChanges();

                return databaseUser.Id;
            }
        }

        // Нахождение пользователя по логину
        static public Entity.User Get(string login)
        {
            using (RaceContext data = new RaceContext())
            {
                var databaseUser = data.Users.FirstOrDefault(x => x.Name == login);
                if (databaseUser != null)
                {
                    var user = new Entity.User();
                    user.Id = databaseUser.Id;
                    user.Name = databaseUser.Name;
                    user.Password = databaseUser.Password;
                    return user;
                }
                return null;
            }
        }
    }
}
