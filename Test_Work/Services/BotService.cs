using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Test_Work.Abstractions;
using Test_Work.Entities;

namespace Test_Work.Services;

public class BotService(ICalculatorService calculatorService, IReminderService reminderService, ITelegramBotClient botClient, IStateService stateService) : IBotService
{
    private async Task ShowMainMenuAsync(long chatId)
    {
        var replyKeyboard = new ReplyKeyboardMarkup(new[]
        {
            new KeyboardButton[] { "1) Калькулятор", "2) Добавить напоминание" },
            new KeyboardButton[] { "3) Список напоминаний", "4) Главное меню" }
        })
        {
            ResizeKeyboard = true
        };

        await botClient.SendTextMessageAsync(
            chatId,
            "Главное меню:",
            replyMarkup: replyKeyboard
        );
    }

    public async Task HandleUpdateAsync(Update update)
    {
        if (update.Message?.Text == "/start")
        {
            var chatId = update.Message.Chat.Id;

            await ShowMainMenuAsync(chatId);

            return;
        }

        if (update.Message?.Text != null)
        {
            var chatId = update.Message.Chat.Id;
            var userMessage = update.Message.Text;

            if (stateService.PendingDeletionReminders.Contains(chatId))
            {
                await HandleDeleteReminderCommandAsync(chatId, userMessage);
                return;
            }

            switch (userMessage)
            {
                case "1) Калькулятор":
                    await HandleCalculateCommandAsync(chatId, "/calculate");
                    return;
                case "2) Добавить напоминание":
                    await HandleAddReminderCommandAsync(chatId, "/addreminder");
                    return;
                case "3) Список напоминаний":
                    await HandleListRemindersCommandAsync(chatId);
                    return;
                case "4) Главное меню":
                    await ShowMainMenuAsync(chatId);
                    return;
                default:
                    if (stateService.ReminderSteps.ContainsKey(chatId))
                    {
                        await HandleAddReminderCommandAsync(chatId, userMessage);
                        return;
                    }

                    if (stateService.PendingCalculations.TryGetValue(chatId, out var isPending) && isPending)
                    {
                        await HandlePendingCalculationAsync(chatId, userMessage);
                        return;
                    }

                    await botClient.SendTextMessageAsync(chatId, "Команда не распознана. Пожалуйста, используйте кнопки меню.");
                    return;
            }
        }
        else if (update.CallbackQuery != null)
        {
            var chatId = update.CallbackQuery.Message.Chat.Id;
            var data = update.CallbackQuery.Data;

            switch (data)
            {
                case "list_reminders":
                    await HandleListRemindersCommandAsync(chatId);
                    break;
                case "delete_reminder":
                    stateService.PendingDeletionReminders.Add(chatId);
                    await botClient.SendTextMessageAsync(chatId, "Пожалуйста, введите ID напоминания, которое хотите удалить.");
                    break;
                case "main_menu":
                    await ShowMainMenuAsync(chatId);
                    break;
                default:
                    await botClient.SendTextMessageAsync(chatId, "Действие не распознано.");
                    break;
            }

            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
        }
    }

    private async Task HandlePendingCalculationAsync(long chatId, string expression)
    {
        var result = calculatorService.Calculate(expression);
        stateService.PendingCalculations.TryRemove(chatId, out _);

        var inlineKeyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Главное меню", "main_menu")
            }
        });

        await botClient.SendTextMessageAsync(chatId, $"Результат: {result}", replyMarkup: inlineKeyboard);
    }

    private async Task HandleCalculateCommandAsync(long chatId, string userMessage)
    {
        var expression = userMessage.Replace("/calculate", "").Trim();

        if (string.IsNullOrEmpty(expression))
        {
            stateService.PendingCalculations[chatId] = true;
            await botClient.SendTextMessageAsync(
                chatId,
                "Пожалуйста, введите математическое выражение (например, 2+2).",
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
        else
        {
            var result = calculatorService.Calculate(expression);
            stateService.PendingCalculations.TryRemove(chatId, out _);

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Главное меню", "main_menu")
                }
            });

            await botClient.SendTextMessageAsync(
                chatId,
                $"Результат: {result}",
                replyMarkup: inlineKeyboard
            );
        }
    }

    private async Task HandleAddReminderCommandAsync(long chatId, string userMessage)
    {
        if (userMessage.Equals("/addreminder", StringComparison.OrdinalIgnoreCase))
        {
            await botClient.SendTextMessageAsync(chatId, "Введите напоминание в формате:\nдд.мм.гг чч:мм описание", replyMarkup: new ReplyKeyboardRemove());
            stateService.ReminderSteps[chatId] = 1;
            return;
        }

        if (stateService.ReminderSteps.TryGetValue(chatId, out var step) && step == 1)
        {
            var input = userMessage.Trim();

            var dateTimeLength = "dd.MM.yy HH:mm".Length;

            if (input.Length <= dateTimeLength)
            {
                await botClient.SendTextMessageAsync(chatId, "Неверный формат. Убедитесь, что вы вводите дату, время и описание.");
                return;
            }

            var dateTimePart = input[..dateTimeLength];
            var descriptionPart = input[dateTimeLength..].Trim();

            const string format = "dd.MM.yy HH:mm";

            if (DateTime.TryParseExact(dateTimePart, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var reminderDate))
            {
                var reminder = new Reminder
                {
                    ChatId = chatId,
                    ReminderDate = reminderDate,
                    Description = descriptionPart
                };

                await reminderService.AddReminderAsync(reminder);

                stateService.ReminderSteps.TryRemove(chatId, out _);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Список напоминаний", "list_reminders"),
                    InlineKeyboardButton.WithCallbackData("Главное меню", "main_menu")
                }
            });

                await botClient.SendTextMessageAsync(
                    chatId,
                    "Напоминание успешно добавлено!",
                    replyMarkup: inlineKeyboard
                );
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Неверный формат даты и времени. Пожалуйста, используйте формат дд.мм.гг чч:мм.");
            }
        }
    }

    private async Task HandleListRemindersCommandAsync(long chatId)
    {
        var reminders = await reminderService.GetActiveRemindersAsync(chatId);

        if (reminders.Any())
        {
            var message = reminders.Aggregate("Ваши предстоящие напоминания:\n",
                (current, reminder) => current + $"{reminder.Id}: {reminder.ReminderDate:dd.MM.yy HH:mm} - {reminder.Description}\n");

            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Удалить напоминание", "delete_reminder"),
                    InlineKeyboardButton.WithCallbackData("Главное меню", "main_menu")
                }
            });

            await botClient.SendTextMessageAsync(chatId, message, replyMarkup: inlineKeyboard);
        }
        else
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Главное меню", "main_menu")
                }
            });

            await botClient.SendTextMessageAsync(chatId, "У вас нет предстоящих напоминаний.", replyMarkup: inlineKeyboard
            );
        }
    }

    private async Task HandleDeleteReminderCommandAsync(long chatId, string userMessage)
    {
        if (int.TryParse(userMessage, out var reminderId))
        {
            var reminder = await reminderService.GetReminderByIdAsync(reminderId);

            if (reminder != null && reminder.ChatId == chatId && !reminder.IsDeleted)
            {
                await reminderService.MarkAsDeletedAsync(reminderId);
                stateService.PendingDeletionReminders.Remove(chatId);
                await botClient.SendTextMessageAsync(chatId, "Напоминание успешно удалено.");

                await ShowMainMenuAsync(chatId);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId, "Напоминание не найдено или уже удалено.");
            }
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, "Некорректный ID. Пожалуйста, введите числовой ID напоминания.");
        }
    }
}
