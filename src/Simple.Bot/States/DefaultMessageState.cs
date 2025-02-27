using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simple.Bot.States;

internal sealed class DefaultMessageState : IState
{
    public async Task<IState> Process(ITelegramBotClient bot, Update update) {
        var task = update switch {
            { Message: {} message } => Task.FromResult<IState>(this),
            { CallbackQuery: {} callbackQuery } => Task.FromResult<IState>(this),
            _ => ProcessUnknown(update)
        };

        return await task;
    }

    private Task<IState> ProcessUnknown(Update udpate) {
        Console.WriteLine($"[info] Get unknown udpate: {udpate.Type}");
        return Task.FromResult<IState>(this);
    }
}
