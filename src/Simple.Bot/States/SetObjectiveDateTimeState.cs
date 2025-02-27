using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Simple.Bot.States;

internal sealed class SetObjectiveDateTimeState : IState
{
    private readonly Objective _objective;

    public SetObjectiveDateTimeState(Objective objective) {
        _objective = objective;
    }

    public async Task<IState> Process(ITelegramBotClient bot, Update update) {
        if (update is not { Message: {} message }) {
            Console.WriteLine($"[info] Incorrect update type. Expected Message, get {update.Type}");
            return this;
        }

        if (!DateTime.TryParse(message.Text, out var dateTime)) {
            await bot.SendMessage(message.Chat, "Неверный формат даты, попробуйте снова");
            return this;
        }

        if (dateTime <= DateTime.Now) {
            await bot.SendMessage(message.Chat, "Дата не может быть раньше текущей, попробуйте снова");
            return this;
        }

        _objective.DateTime = dateTime;
        await bot.SendMessage(
            message.Chat,
            $"Вы хотите создать следующую задачу\nНазвание: {_objective.Name}\nВыполнить до: {_objective.DateTime}",
            replyMarkup: new InlineKeyboardButton[] { "Да", "Нет" });

        return new DefaultMessageState();
    }
}
