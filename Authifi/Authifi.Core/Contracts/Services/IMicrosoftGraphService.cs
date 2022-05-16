using System.Threading.Tasks;

using Authifi.Core.Models;

namespace Authifi.Core.Contracts.Services
{
    public interface IMicrosoftGraphService
    {
        Task<User> GetUserInfoAsync(string accessToken);

        Task<string> GetUserPhoto(string accessToken);
    }
}
