using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Bot.Crypto;
using Bot.NumTrivia;
using Bot.Waifu;
using System.Diagnostics;
using System.Threading;


var botClient = new TelegramBotClient("6556160252:AAE2v3fF_2ogQR5e1ZbwWcw-6pR5LULifiY");

using CancellationTokenSource cts = new();
// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    string commandResponse = "";
    string pic = "";

    if (message.Text.StartsWith("/"))
    {
        // The received message is a command
        string command = message.Text.Split(' ')[0]; // Extract the command
        switch (command.ToLower())
        {
            case "/pricecrypto":
                decimal price = await Crypto.CryptoPrice(message.Text.Split(' ')[1]);
                if (price == 0)
                {
                    commandResponse = """
                        the crytpo symbol you provided are not valid 
                        """;
                }
                else
                {
                    commandResponse = $"{price:C}";
                }
                break;
            case "/num":
                if (message.Text.Length > 4)
                {
                    if (decimal.TryParse(message.Text.Split(' ')[1], out decimal a))
                    {
                        string numTrivia = await NumTrivia.GetNumTrivia(Convert.ToDecimal(message.Text.Split(' ')[1]));
                        commandResponse = numTrivia;
                    }
                    else
                    {
                        commandResponse = "please provide an integer";
                    }
                }
                else
                {
                    commandResponse = "please provide an integer after /num command";
                }


                break;
            case "/waifu":
                pic = await Waifu.GetWaifuURL();

                break;
            default:
                commandResponse = """
                    please provide one of these commands:
                    /cryptoPrice arg:"crypto symbol" => for crypto coin prices
                    /waifu no arg needed => receive a waifu picture
                    /num arg:"a number" => get a fun fact about the number
                    """;
                break;
        }
    }
    else
    {
        commandResponse = """
                    please provide one of these commands:
                    /cryptoPrice arg:"crypto symbol" => for crypto coin prices
                    /waifu no arg needed => receive a waifu picture
                    /num arg:"a number" => get a fun fact about the number
                    """;
    }
    if (commandResponse == "")
    {
        Message picMessage = await botClient.SendPhotoAsync(
    chatId: chatId,
    photo: InputFile.FromUri(pic),
    caption: "",
    parseMode: ParseMode.Html,
    cancellationToken: cancellationToken);
    }
    else
    {
        // Response to received message text

        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: commandResponse,
            cancellationToken: cancellationToken);
    }

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

}


Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
