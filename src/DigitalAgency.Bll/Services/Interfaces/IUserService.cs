using System.Threading.Tasks;

namespace DigitalAgency.Bll.Services.Interfaces
{
    public interface IUserService
    {
        public Task<bool> IsUserExist(string UserPhoneNumber);
        public Task<bool> RegisterUser(string UserPhoneNumber, string UserPassword);
        public Task<string> GetToken(string UserPhoneNumber, string UserPassword);
    }
}
