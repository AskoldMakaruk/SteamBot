using BotFramework.Bot;
using Serilog;

namespace BFTemplate
{
    class Program
    {
        static void Main()
        {
            new BotBuilder()
            .UseAssembly(typeof(Program).Assembly)
            .WithName("EchoBot")
            .WithToken("<YOURTOKEN>")
            .UseLogger(new LoggerConfiguration()
                       .MinimumLevel.Debug()
                       .WriteTo.Console()
                       .CreateLogger())
            .Build()
            .Run();
        }
    }
}
/*
 * amazing guide about webhooks
 * https://core.telegram.org/bots/webhooks
 *
 * Useful commands:
 * -Efcore:
 * -- dotnet ef migrations add initial
 * -- dotnet ef database update
 *
 * Packages
 * dotnet add package Microsoft.EntityFrameworkCore.Tools
 * dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
 * dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL.Design
 *
 *  
 */
