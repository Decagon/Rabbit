using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlayerIOClient;
using System.Text.RegularExpressions;
using Rabbit.Auth;

namespace Rabbit
{
    public static class ArmorGames : Auth
    {
        public static Client Authenticate(string email, string password)
        {
            var c = Utilities.GuestClient.Value.Multiplayer.JoinRoom("", null);
            c.OnMessage += (sender, message) =>
            {
                if (message.Type != "auth") return;

                if (message.Count == 0) {throw new Exception("Could not log into ArmorGames.");}
                else
                {
                    Auth.client = PlayerIOClient.PlayerIO.Connect("everybody-edits-su9rn58o40itdbnw69plyw", "secure",
                                                                  message.GetString(0), message.GetString(1),
                                                                  "armorgames");
                }

                c.Disconnect();
            };

            c.Send("auth", email, password);
        }
    }

}