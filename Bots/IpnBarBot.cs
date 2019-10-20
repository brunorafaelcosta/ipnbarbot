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
using ipnbarbot.Application.Models;
using System.Linq;

namespace ipnbarbot.Bots
{
    public class IpnBarBot<T> : BaseDialogBot<T> where T : Dialog
    {
        private readonly IConfiguration _configuration;
        private readonly Infrastructure.Utils.LocalizationHelper _localizationHelper;

        private readonly Recognizers.MenuRecognizer _menuRecognizer;

        private readonly Application.IUsersApp _usersApp;
        private readonly Application.IMealSchedulesApp _mealSchedulesApp;

        public IpnBarBot(ConcurrentDictionary<string, ConversationReference> conversationReferences,
            ConversationState conversationState, UserState userState, T dialog, ILogger<BaseDialogBot<T>> logger,
            IConfiguration configuration, Infrastructure.Utils.LocalizationHelper localizationHelper,
            Recognizers.MenuRecognizer menuRecognizer,
            Application.IUsersApp usersApp, Application.IMealSchedulesApp mealSchedulesApp)
            : base(conversationReferences, conversationState, userState, dialog, logger)
        {
            this._configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this._localizationHelper = localizationHelper ?? throw new ArgumentNullException(nameof(localizationHelper));
            this._menuRecognizer = menuRecognizer ?? throw new ArgumentNullException(nameof(menuRecognizer));
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
                    
                    if (await this._usersApp.ChannelAccountIdIsRegistered(member.Id) == false)
                        await this._usersApp.RegisterAsync(member.Id, member.Name, userLanguage);

                    string WelcomeMessage = this._localizationHelper.Localize("WelcomeMessage", userLanguage);
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format(WelcomeMessage, member.Name)), cancellationToken);
                }
            }
        }
        
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (!_menuRecognizer.IsConfigured)
            {
                throw new System.InvalidOperationException();
            }

            string language = "pt-PT";

            AddConversationReference(turnContext.Activity as Activity);

            var conversationReference = turnContext.Activity.GetConversationReference();
            var userChannelAccount = conversationReference.User;

            if (await this._usersApp.ChannelAccountIdIsRegistered(userChannelAccount.Id) == false)
                await this._usersApp.RegisterAsync(userChannelAccount.Id, userChannelAccount.Name, language);
            
            try
            {
                User user = await this._usersApp.GetByChannelAccountId(userChannelAccount.Id);
                language = user.Language;

                var luisResult = await _menuRecognizer.RecognizeAsync<Luis.IPNBarMenuCognitiveModel>(turnContext, cancellationToken);
                switch (luisResult.TopIntent().intent)
                {
                    case Luis.IPNBarMenuCognitiveModel.Intent.MenuDiario:
                        DateTime? menuDiarioDate = luisResult.MenuDateAsDateTime;
                        if (menuDiarioDate is null)
                        {
                            throw new Exceptions.DefaultBotException("DidntUnderstantMenuDateAskAgain");
                        }
                        else
                        {
                            var menuDiarioMessageText = await this.GetMenuFor(menuDiarioDate.Value);
                            var menuDiarioMessage = MessageFactory.Text(menuDiarioMessageText, menuDiarioMessageText, InputHints.IgnoringInput);
                            await turnContext.SendActivityAsync(menuDiarioMessage, cancellationToken);
                            // Attachment menuDiarioMessageCard = await this.GetMenuFor(menuDiarioDate.Value);
                            // var menuDiarioMessageCardActivity = MessageFactory.Attachment(menuDiarioMessageCard);
                            // await turnContext.SendActivityAsync(menuDiarioMessageCardActivity, cancellationToken);
                        }
                        break;

                    case Luis.IPNBarMenuCognitiveModel.Intent.MenuSemanal:
                            var menuSemanalMessageText = await this.GetWeekMenu();
                            var menuSemanalMessage = MessageFactory.Text(menuSemanalMessageText, menuSemanalMessageText, InputHints.IgnoringInput);
                            await turnContext.SendActivityAsync(menuSemanalMessage, cancellationToken);
                        // Attachment menuSemanalMessageCard = await this.GetWeekMenu();
                        // var menuSemanalMessageCardActivity = MessageFactory.Attachment(menuSemanalMessageCard);
                        // await turnContext.SendActivityAsync(menuSemanalMessageCardActivity, cancellationToken);
                        break;

                    default:
                        // Catch all for unhandled intents
                        throw new Exceptions.DefaultBotException("DidntUnderstantAskAgain");
                }
            }
            catch (Exceptions.DefaultBotException botException)
            {
                string languageKey = botException.Message;
                var botExceptionMessageText = this._localizationHelper.Localize(languageKey, language);
                var botExceptionMessage = MessageFactory.Text(botExceptionMessageText, botExceptionMessageText, InputHints.IgnoringInput);
                await turnContext.SendActivityAsync(botExceptionMessage, cancellationToken);
            }
        }

        private async Task<string> GetMenuFor(DateTime date)
        {
            string response = null;

            MealSchedule menu = await this._mealSchedulesApp.GetForDate(date);

            if (menu is null)
            {
                throw new Exceptions.DefaultBotException("MenuNotFoundForDate");
            }
            else
            {
                response = ipnbarbot.Cards.CardsExtensions.GetDailyMenuCardAsString(menu);
            }

            return response;
        }
        
        private async Task<string> GetWeekMenu()
        {
            string response = null;

            IEnumerable<MealSchedule> weekMenu = await this._mealSchedulesApp.GetForWeek(Application.Helpers.DateHelpers.Today);

            if (weekMenu is null || weekMenu.Count() < 1)
            {
                throw new Exceptions.DefaultBotException("MenuNotSetForCurrentWeek");
            }
            else
            {
                response = ipnbarbot.Cards.CardsExtensions.GetWeeklyMenuCardAsString(weekMenu);
            }

            return response;
        }
    }
}
