using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace ipnbarbot.Bots
{
    public class BaseDialogBot<T> : ActivityHandler where T : Dialog
    {
        protected readonly ConcurrentDictionary<string, ConversationReference> ConversationReferences;
        // protected readonly Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly ILogger Logger;

        public BaseDialogBot(ConcurrentDictionary<string, ConversationReference> conversationReferences, ConversationState conversationState, UserState userState, T dialog, ILogger<BaseDialogBot<T>> logger)
        {
            ConversationReferences = conversationReferences;
            ConversationState = conversationState;
            UserState = userState;
            // Dialog = dialog;
            Logger = logger;
        }

        // public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        // {
        //     await base.OnTurnAsync(turnContext, cancellationToken);

        //     // Save any state changes that might have occured during the turn.
        //     await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        //     await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        // }

        // protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        // {
        //     Logger.LogInformation("Running dialog with Message Activity.");

        //     AddConversationReference(turnContext.Activity as Activity);

        //     // Run the Dialog with the new message Activity.
        //     await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        // }
        
        // protected override  Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        // {
        //     AddConversationReference(turnContext.Activity as Activity);

        //     return base.OnConversationUpdateActivityAsync(turnContext, cancellationToken);
        // }

        protected virtual void AddConversationReference(Activity activity)
        {
            var conversationReference = activity.GetConversationReference();
            this.ConversationReferences.AddOrUpdate(conversationReference.User.Id, conversationReference, (key, newValue) => conversationReference);
        }
    }
}