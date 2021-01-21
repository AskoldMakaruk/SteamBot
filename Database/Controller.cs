using System;
using System.Linq;
using BFTemplate.Model;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;

namespace BFTemplate.Database
{
    public class Controller
    {
        public Controller()
        {
            Context = new TelegramContext();
        }

        private readonly TelegramContext Context;

#region Account

        public static MemoryCache    cache = new MemoryCache(new MemoryCacheOptions());
        public static DateTimeOffset DefaultCacheOffset => DateTimeOffset.Now.Add(new TimeSpan(0, 10, 0));

        public Account this[long key] { get => GetAccount(key); set => cache.Set(key, value, DefaultCacheOffset); }

        public static Account GetAccount(long chatId)
        {
            var cachedAccount = cache.Get(chatId);
            return cachedAccount as Account;
        }

        public Account GetAccount(Message message)
        {
            var result = GetAccount(message.From.Id);
            if (result != null)
                return result;

            var account = Context.Accounts.FirstOrDefault(a => a.ChatId == message.From.Id) ?? CreateAccount(message);

            cache.Set((long) message.From.Id, account, DefaultCacheOffset);

            return account;
        }

        private Account CreateAccount(Message message)
        {
            var account = new Account
            {
                ChatId   = message.Chat.Id,
                Username = message.Chat.Username
            };
            if (message.Chat.Username == null)
                account.Username = message.Chat.FirstName + " " + message.Chat.LastName;
            Context.Accounts.Add(account);
            SaveChanges();
            return account;
        }

#endregion

        public void SaveChanges() => Context.SaveChanges();
    }
}