using System.Net;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    // parse query parameter
    string message = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "message", true) == 0)
        .Value;

         // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    message = message ?? data?.message;

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

            ConnectorClient msgConnector = new ConnectorClient(new Uri(conversationServiceURL));
            IMessageActivity reply = Activity.CreateMessageActivity();
            reply.Text = message;
            reply.Locale = "en-Us";
            reply.From = new ChannelAccount("sip:nameofbot@domain.com", "BotName");

            await msgConnector.Conversations.SendToConversationAsync((Activity)reply, conversationID);



return req.CreateResponse(HttpStatusCode.OK, "Done");

}
