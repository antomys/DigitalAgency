using System.Threading.Tasks;

namespace DigitalAgency.Dal.Storages.Interfaces;

public interface IUserStorage
{
    public Task<bool> IsUserExist(string UserPhoneNumber);
    public Task<bool> RegisterUser(string UserPhoneNumber, string UserPassword);
    public Task<string> GetToken(string UserPhoneNumber, string UserPassword);
}