using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.Configuration;

namespace ipnbarbot.Bots
{
    public class IpnBarBot<T> : BaseDialogBot<T> where T : Dialog
    {
        private readonly IConfiguration _configuration;
        private readonly Infrastructure.Utils.LocalizationHelper _localizationHelper;

        private readonly Application.IUsersApp _usersApp;
        private readonly Application.IMealSchedulesApp _mealSchedulesApp;

        public IpnBarBot(ConcurrentDictionary<string, ConversationReference> conversationReferences,
            ConversationState conversationState, UserState userState, T dialog, ILogger<BaseDialogBot<T>> logger,
            IConfiguration configuration, Infrastructure.Utils.LocalizationHelper localizationHelper,
            Application.IUsersApp usersApp, Application.IMealSchedulesApp mealSchedulesApp)
            : base(conversationReferences, conversationState, userState, dialog, logger)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._localizationHelper = localizationHelper ?? throw new ArgumentNullException(nameof(localizationHelper));
            this._usersApp = usersApp ?? throw new ArgumentNullException(nameof(usersApp));
            this._mealSchedulesApp = mealSchedulesApp ?? throw new ArgumentNullException(nameof(mealSchedulesApp));
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    string userLanguage = "pt-PT";
                    await this._usersApp.RegisterAsync(member.Id, member.Name, userLanguage);

                    string WelcomeMessage = this._localizationHelper.Localize("WelcomeMessage", userLanguage);
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(WelcomeMessage, member.Name)), cancellationToken);
                }
            }
        }
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            AddConversationReference(turnContext.Activity as Activity);

            await OnMessageActivityAsync(turnContext, cancellationToken);
        }
    }
}
