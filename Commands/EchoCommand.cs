using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace BFTemplate.Commands
{
    [StaticCommand]
    public class EchoCommand : MessageCommand
    {
        public override Response Execute(Message message, Client client)
        {
            return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
        }

        public override bool Suitable(Message message) => true;
    }
}