using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot.Types;

namespace SteamBot.Commands
{
	public abstract class StaticCommand : IStaticCommand
	{
		public virtual bool SuitableFirst(Update message) => false;
		public virtual bool SuitableLast(Update message) => false;
		public abstract Task Execute(IClient client);
	}
}