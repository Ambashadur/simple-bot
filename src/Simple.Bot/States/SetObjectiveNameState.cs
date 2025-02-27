using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simple.Bot.States;

internal sealed class SetObjectiveNameState : IState
{
    private readonly Objective _objective;

    public SetObjectiveNameState(Objective objective) {
        _objective = objective;
    }

    public async Task<IState> Process(ITelegramBotClient bot, Update update) {
        if (update is not { Message: {} message }) {
            Console.WriteLine($"[info] Incorrect update type. Expected Message, get {update.Type}");
            return this;
        }

        _objective.Name = message.Text;
        await bot.SendMessage(
                message.Chat,
                $"Название '{message.Text}' успешно задано.\nВведите дату, к которой должна быть выполнена данная задача");
        
        return new SetObjectiveDateTimeState(_objective);
    }
}
