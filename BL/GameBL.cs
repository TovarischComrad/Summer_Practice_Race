using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public class GameBL
    {
        static public int AddOrUpdate(Entity.Game game)
        {
            return DAL.GameDAL.AddOrUpdate(game);
        }

        static public Entity.Game Get(int id)
        {
            return DAL.GameDAL.Get(id);
        }

        static public Entity.SearchOut.SearchOutGame Get(Entity.SearchIn.SearchingGame query)
        {
            return DAL.GameDAL.Get(query);
        }
    }
}
