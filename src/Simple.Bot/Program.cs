using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Simple.Bot.FoodClient;
using Simple.Bot.States;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Simple.Bot;

internal class Program
{
    private const string PATH = "./objectives.json";

    private static TelegramBotClient _bot = null!;
    private static Dictionary<long, Objective> _upcomingObjectives = new Dictionary<long, Objective>();
    private static Dictionary<long, IState> _chatStates = new Dictionary<long, IState>();

    private static async Task Main(string[] args) {
        using var cts = new CancellationTokenSource();

        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<Program>()
            .Build();

        SpoonacularClient.ApiKey = configuration["Spoonacular:ApiKey"];
        _bot = new TelegramBotClient(configuration["Telegram:Token"] ?? string.Empty, cancellationToken: cts.Token);

        var me = await _bot.GetMe();
        var commands = new BotCommand[] {
            new() { Command = "hello", Description = "Просто сказать привет" },
            new() { Command = "addobjective", Description = "Добавить новую задачу" },
            new() { Command = "listobjectives", Description = "Показать список всех задач" },
            new() { Command = "searchrecipe", Description = "Поиск рецептов по названиям" },
        };

        await _bot.SetMyCommands(commands);

        Repository.Initialize(PATH);

        _bot.OnError += OnError;
        _bot.OnUpdate += OnUpdate;

        Console.WriteLine($"{me.Username} is running... Press Enter to terminate");
        Console.ReadLine();
        cts.Cancel();
    }

    private static Task OnError(Exception exception, HandleErrorSource source) {
        Console.WriteLine($"[error] Get error {exception} from source {source}");
        return Task.CompletedTask;
    }

    private static Task OnUpdate(Update update) => update switch {
        { Message: {} message } => ProcessCommand(message),
        { CallbackQuery: {} callbackQuery } => ProcessCallbackQuery(callbackQuery),
        _ => ProcessUnknown(update)
    };

    private static Task ProcessCommand(Message message) => message.Text switch {
        "/start" => Start(message),
        "/addobjective" => AddObjective(message),
        "/listobjectives" => ListObjectives(message),
        "/hello" => _bot.SendMessage(message.Chat, "Привет!"),
        "/searchrecipe" => Task.CompletedTask,
        _ => ProcessTextMessage(message),
    };

    private static Task Start(Message message) {
        _chatStates[message.Chat.Id] = new DefaultMessageState();
        return Task.CompletedTask;
    }

    private static async Task AddObjective(Message message) {
        Console.WriteLine($"[info] Start adding new objective for {message.Chat}");

        _upcomingObjectives[message.Chat.Id] = new Objective() { ChatId = message.Chat.Id };

        await _bot.SendMessage(message.Chat, "Давайте создадим новую задачу.\nВведите название для задачи");
    }

    private static async Task ListObjectives(Message message) {
        Console.WriteLine($"[info] List all objectives for {message.Chat}");

        var objectives = Repository.Load<Objective>(PATH);
        var strObjectives = objectives
            .Where(obj => obj.ChatId == message.Chat.Id)
            .Select((obj, index) => $"Задача {index + 1}\n- Название: {obj.Name}\n- Выполнить до: {obj.DateTime}");

        var answer = string.Join('\n', strObjectives);

        if (string.IsNullOrEmpty(answer)) {
            answer = "Нет задач";
        }

        await _bot.SendMessage(message.Chat, answer);
    }

    private static async Task ProcessTextMessage(Message message) {
        Console.WriteLine($"[info] Process text message for {message.Chat}");

        if (!_upcomingObjectives.TryGetValue(message.Chat.Id, out var objective)) {
            await _bot.SendMessage(message.Chat, "Чего?");
            return;
        }

        if (objective.Name is null) {
            objective.Name = message.Text;
            await _bot.SendMessage(
                message.Chat,
                $"Название '{message.Text}' успешно задано.\nВведите дату, к которой должна быть выполнена данная задача");
        } else if (objective.DateTime is null) {
            if (!DateTime.TryParse(message.Text, out var dateTime)) {
                await _bot.SendMessage(message.Chat, "Неверный формат даты, попробуйте снова");
                return;
            }

            if (dateTime <= DateTime.Now) {
                await _bot.SendMessage(message.Chat, "Дата не может быть раньше текущей, попробуйте снова");
                return;
            }

            objective.DateTime = dateTime;
            await _bot.SendMessage(
                message.Chat,
                $"Вы хотите создать следующую задачу\nНазвание: {objective.Name}\nВыполнить до: {objective.DateTime}",
                replyMarkup: new InlineKeyboardButton[] { "Да", "Нет" });
        }
    }

    private static async Task ProcessCallbackQuery(CallbackQuery query) {
        Console.WriteLine($"[info] Process callback query for user {query.From}");

        if (query.Data == "Да") {
            var objective = _upcomingObjectives[query.Message!.Chat.Id];
            Repository.Store(PATH, objective);
            await _bot.AnswerCallbackQuery(query.Id, "Задача была создана");

        } else if (query.Data == "Нет") {
            await _bot.AnswerCallbackQuery(query.Id, "Создание задачи было отмененно");
        }

        _upcomingObjectives.Remove(query.Message!.Chat.Id);
        await _bot.DeleteMessage(query.Message!.Chat, query.Message!.Id);
    }

    private static Task ProcessUnknown(Update udpate) {
        Console.WriteLine($"[info] Get unknown udpate: {udpate.Type}");
        return Task.CompletedTask;
    }
}
