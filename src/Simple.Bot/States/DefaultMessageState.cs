using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Simple.Bot.States;

internal sealed class DefaultMessageState : IState
{
    private const string PATH = "./objectives.json";

    public async Task<IState> Process(ITelegramBotClient bot, Update update) {
        if (update is not { Message: {} message }) {
            Console.WriteLine($"[info] Get unknown udpate: {update.Type}");
            return this;
        }

        return message.Text switch {
            "/hello" => await Hello(bot, message),
            // "/addobjective" => AddObjective(bot, message),
            "/listobjectives" => await ListObjectives(bot, message),
            _ => this
        };


    }

    private async Task<IState> Hello(ITelegramBotClient bot, Message message) {
        await bot.SendMessage(message.Chat, "Привет!");
        return this;
    }

    private async Task<IState> ListObjectives(ITelegramBotClient bot, Message message) {
        Console.WriteLine($"[info] List all objectives for {message.Chat}");

        var objectives = Repository.Load<Objective>(PATH);
        var strObjectives = objectives
            .Where(obj => obj.ChatId == message.Chat.Id)
            .Select((obj, index) => $"Задача {index + 1}\n- Название: {obj.Name}\n- Выполнить до: {obj.DateTime}");

        var answer = string.Join('\n', strObjectives);

        if (string.IsNullOrEmpty(answer)) {
            answer = "Нет задач";
        }

        await bot.SendMessage(message.Chat, answer);

        return this;
    }
}
