using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simple.Bot.States;

public interface IState
{
    Task<IState> Process(ITelegramBotClient bot, Update update);
}
