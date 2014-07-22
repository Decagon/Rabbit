using PlayerIOClient;

namespace Rabbit.Auth
{
    public static class Kongregate
    {
        public static Client Authenticate(string email, string password)
        {
            return PlayerIO.QuickConnect.KongregateConnect(Rabbit.GameId, email, password);
        }
    }
}