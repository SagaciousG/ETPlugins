using System.Linq;

namespace ET.Server
{
    [FriendOf(typeof(PlayerComponent))]
    public static class PlayerComponentSystem
    {
        public static async ETTask<Player> Add(this PlayerComponent self, string account)
        {
            var dbComponent = self.DomainScene().GetComponent<DBComponent>();
            Player player = null;
            var players = await dbComponent.Query<Player>((a) => a.Account == account);
            if (players.Count == 0)
            {
                var playerId = IdGenerater.Instance.GenerateUnitId(self.DomainZone());
                player = self.AddChildWithId<Player, string>(playerId, account);
                dbComponent.Save(player).Coroutine();
            }
            else
            {
                player = players[0];
                self.AddChild(player);
            }
            
            self.idPlayers.Add(player.Id, player);
            return player;
        }

        public static Player GetByAccount(this PlayerComponent self, string account)
        {
            foreach (var kv in self.idPlayers)
            {
                if (kv.Value.Account == account)
                    return kv.Value;
            }

            return null;
        }
        
        public static Player Get(this PlayerComponent self, long id)
        {
            self.idPlayers.TryGetValue(id, out Player gamer);
            return gamer;
        }

        public static void Remove(this PlayerComponent self, long id)
        {
            self.RemoveChild(id);
            self.idPlayers.Remove(id);
        }

        public static Player[] GetAll(this PlayerComponent self)
        {
            return self.idPlayers.Values.ToArray();
        }
    }
}