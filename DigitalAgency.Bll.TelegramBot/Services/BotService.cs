using System;
using System.Threading.Tasks;
using DigitalAgency.Bll.TelegramBot.Services.Interfaces;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Storages.Interfaces;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace DigitalAgency.Bll.TelegramBot.Services;

public class BotService : IBotService
{
    readonly IClientMenu _clientMenu;
    readonly IClientStorage _clientStorage;
    readonly IExecutorMenu _executorMenu;
    readonly IExecutorStorage _executorStorage;
    readonly IRegistrationService _registrationService;
    bool _isSuccessSingUp;


    public BotService(
        IClientStorage clientStorage,
        IClientMenu clientMenu,
        IExecutorMenu executorMenu,
        IExecutorStorage executorStorage,
        IRegistrationService registrationService)
    {
        _clientStorage = clientStorage;
        _clientMenu = clientMenu;
        _executorMenu = executorMenu;
        _executorStorage = executorStorage;
        _registrationService = registrationService;
    }

    public async Task ProcessMessageAsync(Update update)
    {
        Client thisClient;
        Executor thisExecutor;
        switch (update.Type)
        {
            case UpdateType.Unknown:
                return;
            case UpdateType.CallbackQuery:
            {
                thisClient =
                    await _clientStorage.GetClientAsync(client => client.TelegramId == update.CallbackQuery.From.Id);
                thisExecutor =
                    await _executorStorage.GetExecutorAsync(client => client.TelegramId == update.CallbackQuery.From.Id);
                await MenuSelector(thisClient, thisExecutor, update);
                break;
            }
            case UpdateType.Message:
            {
                thisClient =
                    await _clientStorage.GetClientAsync(client => client.TelegramId == update.Message.From.Id);
                thisExecutor =
                    await _executorStorage.GetExecutorAsync(client => client.TelegramId == update.Message.From.Id);

                Message receivedMessage = update.Message;

                if (receivedMessage == null)
                    throw new ArgumentNullException($"{nameof(receivedMessage)} is null! [if (receivedMessage == null) BOTSERVICE]");

                if (thisExecutor?.FirstName == null
                    || thisClient?.FirstName == null)
                {
                    if (thisClient != null && thisClient.ChatId != 0
                        || thisExecutor != null && thisExecutor.ChatId != 0)
                    {
                        await MenuSelector(thisClient, thisExecutor, update);
                    }
                    else
                    {
                        _isSuccessSingUp = await _registrationService.StartRegistration(update);
                    }
                }
                else
                {
                    await MenuSelector(thisClient, thisExecutor, update);
                }

                // Just to ensure we always will get into this menu selector
                if (_isSuccessSingUp)
                {
                    thisClient = await _clientStorage.GetClientAsync(x => x.TelegramId == update.Message.From.Id);
                    thisExecutor = await _executorStorage.GetExecutorAsync(x => x.TelegramId == update.Message.From.Id);
                    await MenuSelector(thisClient, thisExecutor, update);
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    async Task MenuSelector(Client thisClient, Executor thisExecutor, Update update)
    {
        if (thisExecutor != null && thisClient == null)
        {
            if (update.CallbackQuery != null)
            {
                await _executorMenu.ProcessCallBack(thisExecutor, update);
            }
            else if (update.Message.ReplyToMessage != null)
            {
                throw new NotImplementedException();
            }
            else
            {
                await _executorMenu.StartMenu(thisExecutor, update);
            }
        }
        else if (thisExecutor == null && thisClient != null)
        {
            if (update.CallbackQuery != null)
            {
                await _clientMenu.ProcessCallBack(thisClient, update);
            }
            else if (update.Message.ReplyToMessage != null)
            {
                await _clientMenu.ProcessReplyToMessage(thisClient, update);
            }
            else
            {
                //todo: client menu
                await _clientMenu.ClientMainMenu(thisClient, update);
            }
        }
        else
        {
            await _registrationService.StartRegistration(update);
        }
    }
}