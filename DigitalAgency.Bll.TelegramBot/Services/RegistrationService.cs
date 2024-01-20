using System;
using System.Text;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DigitalAgency.Bll.TelegramBot.Services;

public class RegistrationService : IRegistrationService
{
    readonly IClientStorage _clientStorage;
    readonly IExecutorStorage _executorStorage;
    readonly ITelegramBotClient _telegram;

    public RegistrationService(
        IClientStorage clientStorage,
        IExecutorStorage executorStorage,
        ITelegramBotClient telegram)
    {
        _clientStorage = clientStorage;
        _executorStorage = executorStorage;
        _telegram = telegram;
    }

    public async Task<bool> StartRegistration(Update update)
    {
        Message receivedMessage = update.Message;

        if (update.Message.Contact != null)
        {
            await WelcomeMessage(update.Message);
            return false;
        }
        if (update.Message.Contact == null && receivedMessage == null
            || !receivedMessage.Text.Contains("register", StringComparison.OrdinalIgnoreCase))
        {
            await GetContactPhone(update.Message.Chat);
            return false;
        }

        switch (receivedMessage.Text.ToLower())
        {
            case "register as client":
            {
                if (await CreateBotClient(receivedMessage))
                {
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Successfully Signed up!");
                    return true;
                }
                await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Something went wrong. Try again!");
                await WelcomeMessage(receivedMessage);
                return false;
            }
            case "register as executor":
            {
                if (await CreateBotExecutor(receivedMessage))
                {
                    await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Successfully Signed up!");

                    //todo: not return but redirect to executor menu
                    return true;
                }
                await _telegram.SendTextMessageAsync(receivedMessage.Chat, "Something went wrong. Try again!");
                await WelcomeMessage(receivedMessage);
                return false;
            }
            default:
            {
                await _telegram.SendTextMessageAsync(update.Message.Chat.Id, "Wrong command! Try again:)");
                await WelcomeMessage(update.Message);
                return false;
            }
        }
    }

    async Task WelcomeMessage(Message chat)
    {
        await _clientStorage.CreateClientAsync(new Client {
            TelegramId = chat.Contact.UserId.Value, PhoneNumber = GetPhoneNumber(chat),
        });

        const string regAs = "Register as ";
        ReplyKeyboardMarkup keyboard = KeyboardMessages.DefaultKeyboardMessage(new[] {
            regAs + "Client",
            regAs + "Executor",
        });

        await _telegram.SendTextMessageAsync(chat.Chat,
            "Now please choose who you want to be: ", replyMarkup: keyboard);
    }

    async Task GetContactPhone(Chat chat)
    {
        ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(new[] {
            new[] {
                new KeyboardButton("Share your contact") {
                    RequestContact = true,
                },
            },
        }) {
            ResizeKeyboard = true, OneTimeKeyboard = true,
        };
        await _telegram.SendTextMessageAsync(chat,
            "Welcome to the Digital Agency bot! Looks like you are new here!\n"
            + "Let's sign you up.\nPlease share your phone.\nIt need to complete registration", replyMarkup: keyboard);
    }

    static string GetPhoneNumber(Message message)
    {
        return message.Contact.PhoneNumber.Contains('+') ? message.Contact.PhoneNumber : message.Contact.PhoneNumber.Insert(0, "+");
    }

    async Task<bool> CreateBotClient(Message message)
    {
        User contactDetails = message.From;
        Client thisContact = await _clientStorage.GetClientAsync(x => x.TelegramId == contactDetails.Id);

        if (thisContact == null) { return false; }

        thisContact.FirstName = contactDetails.FirstName;
        thisContact.MiddleName = contactDetails.Username;
        thisContact.LastName = contactDetails.LastName;
        thisContact.PhoneNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(thisContact.PhoneNumber));
        thisContact.TelegramId = thisContact.TelegramId;
        thisContact.ChatId = message.Chat.Id;

        await _clientStorage.UpdateClientAsync(thisContact);

        return true;
    }

    async Task<bool> CreateBotExecutor(Message message)
    {
        User contactDetails = message.From;
        Client clientToExecutor = await _clientStorage.GetClientAsync(x => x.TelegramId == contactDetails.Id);

        if (clientToExecutor == null) return false;

        Executor executor = new Executor {
            FirstName = contactDetails.FirstName,
            MiddleName = contactDetails.Username,
            LastName = contactDetails.LastName,
            PhoneNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientToExecutor.PhoneNumber)),
            Position = PositionsEnum.Unknown,
            TelegramId = clientToExecutor.TelegramId,
            ChatId = message.Chat.Id,
        };
        await _clientStorage.DeleteClientAsync(clientToExecutor.Id);
        await _executorStorage.CreateExecutorAsync(executor);
        return true;
    }
}