using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ipnbarbot.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ipnbarbot.Application
{
    public class UsersApp : IUsersApp
    {
        #region IDisposable
        public void Dispose()
        {
        }
        #endregion

        private readonly IConfiguration _configuration;
        private readonly Data.ApplicationDbContext _dbContext;

        public UsersApp(IConfiguration configuration, Data.ApplicationDbContext dbContext)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task RegisterAsync(string channelAccountId, string name, string language)
        {
            if (channelAccountId is null || string.IsNullOrEmpty(channelAccountId.Trim()))
                throw new ArgumentNullException(nameof(channelAccountId));
            else if (name is null || string.IsNullOrEmpty(name.Trim()))
                throw new ArgumentNullException(nameof(name));
            else if (language is null || string.IsNullOrEmpty(language.Trim()))
                throw new ArgumentNullException(nameof(language));
            if (this._dbContext.Users.Any(u => u.ChannelAccountId == channelAccountId))
                throw new Exceptions.ApplicationHandledException("Channel account id already registered!");

            if (new string[] { "pt-PT", "en-US" }.Any(l => l == language) == false)
            {
                language = "pt-PT";
            }

            Models.User newUser = new Models.User()
            {
                ChannelAccountId = channelAccountId,
                Name = name,
                Language = language,
                JoinDate = DateTime.Now
            };
            
            await this._dbContext.Users.AddAsync(newUser);

            await this._dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await this._dbContext.Users.AsNoTracking().ToListAsync();
        }

        public async Task<User> GetByChannelAccountId(string id)
        {
            User user = await this._dbContext.Users.FirstOrDefaultAsync(u => u.ChannelAccountId == id);

            if (user is null)
                throw new Exceptions.ApplicationHandledException($"User not found channel account id: {id}");
            
            return user;
        }

        public async Task<bool> ChannelAccountIdIsRegistered(string id)
        {
            return await this._dbContext.Users.AnyAsync(u => u.ChannelAccountId == id);
        }
    }
}
