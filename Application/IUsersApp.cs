using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ipnbarbot.Application
{
    public interface IUsersApp : IDisposable
    {
        Task RegisterAsync(string channelAccountId, string name, string language);

        Task<IEnumerable<Models.User>> GetAll();

        Task<Models.User> GetByChannelAccountId(string id);
    }
}