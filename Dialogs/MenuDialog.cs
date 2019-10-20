using System;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;

namespace ipnbarbot.Dialogs
{
    public class MenuDialog : ComponentDialog
    {
        protected readonly ILogger Logger;

        public MenuDialog(ILogger<MenuDialog> logger)
            : base(nameof(MenuDialog))
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            // {
            //     ActStepAsync
            // }));

            // // The initial child Dialog to run.
            // InitialDialogId = nameof(WaterfallDialog);
        }
    }
}
