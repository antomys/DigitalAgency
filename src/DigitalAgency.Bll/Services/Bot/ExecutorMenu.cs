using DigitalAgency.Bll.Services.Bot.Interfaces;

namespace DigitalAgency.Bll.Services.Bot
{
    public class ExecutorMenu : IExecutorMenu
    {
        public ExecutorMenu()
        {
            
        }
        /*
        await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
            $"Mechanic menu",replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsMechanic));
                        
        if (receivedMessage.Text == null)
    {
        await _telegram.SendTextMessageAsync(receivedMessage.Chat.Id,
            $"Wrong operation",
            replyMarkup: KeyboardMessages.DefaultKeyboardMessage(_botConfiguration.CommandsMechanic));
        return;
    }

    switch (receivedMessage.Text.ToLower())
    {
        case "view my orders":
        {
            var orders = await _orderService.GetExecutorOrdersAsync(thisClient.Id);
            await ViewOrders(orders, receivedMessage);
            return;
        }
    }
    return;
    */

    }
}