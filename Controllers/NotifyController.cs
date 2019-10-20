using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;

namespace ipnbarbot.Controllers
{
    [Route("api/notify")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly string _appId;
        private readonly ConcurrentDictionary<string, ConversationReference> _conversationReferences;

        public NotifyController(IBotFrameworkHttpAdapter adapter, IConfiguration configuration, ConcurrentDictionary<string, ConversationReference> conversationReferences)
        {
            _adapter = adapter;
            _conversationReferences = conversationReferences;
            _appId = configuration["MicrosoftAppId"];

            // If the channel is the Emulator, and authentication is not in use,
            // the AppId will be null.  We generate a random AppId for this case only.
            // This is not required for production, since the AppId will have a value.
            if (string.IsNullOrEmpty(_appId))
            {
                _appId = Guid.NewGuid().ToString(); //if no AppId, use a random Guid
            }
        }

        // [Route("All")]
        // [HttpGet]
        // [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        // [ProducesResponseType((int)HttpStatusCode.NotFound)]
        // [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        // [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        // public async Task<IActionResult> All()
        // {
        //     foreach (var conversationReference in _conversationReferences.Values)
        //     {
        //         await ((BotAdapter)_adapter).ContinueConversationAsync(_appId, conversationReference, BotCallback, default(CancellationToken));
        //     }

        //     return Ok("Done!");
        // }

        // private async Task BotCallback(ITurnContext turnContext, CancellationToken cancellationToken)
        // {
        //     await turnContext.SendActivityAsync("Proactive hello :D");
        // }
    }
}
