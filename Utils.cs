using Life;
using Life.DB;
using Life.Network;

namespace MyBiz
{
    abstract class Utils
    {
        public static bool PlayerIsBizOwner(Player player)
        {
            if (player.HasBiz())
            {
                Bizs biz = Nova.biz.FetchBiz(player.character.BizId);
                return biz.OwnerId == player.character.Id ? true : false;
            } else return false;
        }
    }
}
