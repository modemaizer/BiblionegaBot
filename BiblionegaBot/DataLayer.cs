using BiblionegaBot.Anounces;
using LiteDB;
using Microsoft.Extensions.Logging;
using System;
using Telegram.Bot.Types;

namespace BiblionegaBot
{    
    public class DataLayer : IDataLayer
    {
        private readonly string _databasePath;
        private readonly ILogger<DataLayer> _logger;

        public DataLayer(ILogger<DataLayer> logger, string databasePath)
        {
            _logger = logger;
            _databasePath = databasePath;
        }

        public Anounce GetLastStoredAnounce()
        {
            try
            {
                using var db = new LiteDatabase(_databasePath);
                var anounces = db.GetCollection<Anounce>("anounces");
                return anounces.FindOne(Query.All(Query.Descending));
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Getting last stored anounce failed");
                return null;
            }
        }

        public bool SaveAnounce(Anounce anounce)
        {
            if (anounce == null)
            {
                return false;
            }

            try
            {
                using var db = new LiteDatabase(_databasePath);
                var anounces = db.GetCollection<Anounce>("anounces");
                if (!anounces.Exists(a => a.Id == anounce.Id))
                {
                    anounces.Insert(anounce);
                    return true;
                }
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Saving anounce {AnounceId} failed", anounce.Id);
            }

            return false;
        }

        public bool SaveUser(User user)
        {
            if (user == null)
            {
                return false;
            }

            try
            {
                using var db = new LiteDatabase(_databasePath);
                var users = db.GetCollection<User>("users");
                if (!users.Exists(u => u.Id == user.Id))
                {
                    users.Insert(user);
                    return true;
                }
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Saving user {UserId} failed", user.Id);
            }

            return false;
        }
    }
}
