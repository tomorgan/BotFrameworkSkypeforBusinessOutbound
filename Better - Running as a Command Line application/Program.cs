using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        public static void Main(string[] args)
        {
            CreateConversationAndSend().Wait();
        }

        static async Task CreateConversationAndSend()
        {
            const string trustServiceUri = "https://api.skypeforbusiness.com/platformservice/botframework";
            MicrosoftAppCredentials.TrustServiceUrl(trustServiceUri);
            var to = "sip:tom@thoughtstuff.co.uk";
            var connector = new ConnectorClient(new Uri(trustServiceUri));
            List<ChannelAccount> participants = new List<ChannelAccount>();
            participants.Add(new ChannelAccount(to, to));
            ConversationParameters cpMessage = new ConversationParameters(true, new ChannelAccount("sip:nameofbot@domain.com", "BotName"), participants, "My Test Conversation");
            ConversationResourceResponse response = await connector.Conversations.CreateConversationAsync(cpMessage);
            var conversationID = response.Id;
            var conversationServiceURL = response.ServiceUrl;

            for (int i = 0; i < 10; i++)
            {
                await SendMessage(response.ServiceUrl, $"{i} - The time is {DateTime.UtcNow.ToLongTimeString()}", conversationID);
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static async Task SendMessage(string serviceURL, string message, string conversationID)
        {
            ConnectorClient connector = new ConnectorClient(new Uri(serviceURL));
            IMessageActivity reply = Activity.CreateMessageActivity();
            reply.Text = message;
            reply.Locale = "en-Us";
            reply.From = new ChannelAccount("sip:nameofbot@domain.com", "BotName");

            await connector.Conversations.SendToConversationAsync((Activity)reply, conversationID);
        }
    }
}
