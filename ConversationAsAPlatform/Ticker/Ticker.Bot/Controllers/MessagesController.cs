using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Ticker.Bot.Services;

namespace Ticker.Bot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                string symbol = await GetLastSymbol(activity);
                bool saveSymbol = false;
                string replyMessageText = String.Empty;

                LuisMessage luisMessage = await LuisClient.ParseMessage(activity.Text);

                if (luisMessage.intents.Count() > 0)
                {
                    switch(luisMessage.intents[0].intent)
                    {
                        case "GetStockPrice":
                            replyMessageText = await GetStockPrice(luisMessage.entities[0].entity);
                            symbol = luisMessage.entities[0].entity;
                            saveSymbol = true;
                            break;

                        case "RepeatStockPrice":
                            if (symbol == null)
                            {
                                replyMessageText = "I don't have a previous symbol to look up.";
                            }
                            else
                            {
                                replyMessageText = await GetStockPrice(symbol);
                            }
                            break;

                        default:
                            replyMessageText = "I'm afraid I didn't catch that.";
                            break;
                    }
                }
                else
                {
                    replyMessageText = "I'm afraid I didn't catch that.";
                }

                if (saveSymbol == true)
                {
                    SetLastSymbol(activity, symbol);
                }

                Activity reply = activity.CreateReply(replyMessageText);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<string> GetStockPrice(string symbol)
        {
            double? stockPrice = await StockClient.GetStockPriceAsync(symbol);
            if (stockPrice == null)
            {
                return $"{symbol.ToUpper()} doesn't appear to be a valid stock symbol.";
            }
            else
            {
                return $"The current value of {symbol.ToUpper()} is ${stockPrice.ToString()}";
            }
        }

        private async Task<string> GetLastSymbol(Activity activity)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            return userData.GetProperty<string>("LastSymbol");
        }

        private async void SetLastSymbol(Activity activity, string symbol)
        {
            StateClient stateClient = activity.GetStateClient();
            BotData userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            userData.SetProperty<string>("LastSymbol", symbol);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}