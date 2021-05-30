using System;
using System.IO;
using System.Threading.Tasks;
using Telegram.Bot;

namespace DigitalAgency.Bll.TelegramBot.Services.Helpers
{
    public class DownloadFileExtension
    {
        private readonly ITelegramBotClient _telegram;
        private static readonly string Path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), @"Upload/files");

        public DownloadFileExtension(ITelegramBotClient telegram)
        {
            _telegram = telegram;
        }
        public async Task<string> DownloadFile(string fileId)
        {
            try
            {
                var file = await _telegram.GetFileAsync(fileId);
                var path = System.IO.Path.Combine(Path, System.IO.Path.GetFileName(file.FilePath)!);

                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);
                await using var saveImageStream = new FileStream(path, FileMode.OpenOrCreate);
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
}