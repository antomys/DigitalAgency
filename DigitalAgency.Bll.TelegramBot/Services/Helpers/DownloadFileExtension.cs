using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;
using File = Telegram.Bot.Types.File;

namespace DigitalAgency.Bll.TelegramBot.Services.Helpers;

public class DownloadFileExtension
{
    static readonly string Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Upload/files");
    readonly ITelegramBotClient _telegram;

    public DownloadFileExtension(ITelegramBotClient telegram)
    {
        _telegram = telegram;
    }

    public async Task<string> DownloadFile(string fileId)
    {
        try
        {
            File file = await _telegram.GetFileAsync(fileId);
            string path = System.IO.Path.Combine(Path, System.IO.Path.GetFileName(file.FilePath)!);

            if (!Directory.Exists(Path))
                Directory.CreateDirectory(Path);
            await using FileStream saveImageStream = new FileStream(path, FileMode.OpenOrCreate);
            await _telegram.DownloadFileAsync(file.FilePath, saveImageStream);

            return path;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error downloading: " + ex.Message);
        }

        return null;
    }
}