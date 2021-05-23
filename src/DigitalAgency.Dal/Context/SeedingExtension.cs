using System.Linq;
using System.Threading.Tasks;
using DigitalAgency.Dal.Entities;
using DigitalAgency.Dal.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace DigitalAgency.Dal.Context
{
    /// <summary>
    /// Seeding extension.
    /// </summary>
    public class SeedingExtension
    {
        /// <summary>
        /// First database population
        /// </summary>
        /// <param name="context"></param>
        public static async Task PopulateDatabase(ServicingContext context)
        {
            await EnsureAdminCreated(context);
        }
        private static async Task EnsureAdminCreated(ServicingContext context)
        {
            var adminUser = await context.Executors.AnyAsync(x=> x.FirstName == "NULL");
            if (!adminUser)
            {
                context.Executors.Add(new Executor
                {
                    FirstName = "NULL",
                    MiddleName = "NULL",
                    LastName = "NULL",
                    PhoneNumber = "NULL",
                    Position = PositionsEnum.Unknown,
                    TelegramId = 0,
                    ChatId = 0
                });
            }
            await context.SaveChangesAsync();
        }
    }
}