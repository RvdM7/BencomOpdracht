using System.Net.Http;
using System.Threading.Tasks;

namespace BencomOpdracht.Models
{
    /*
     * Class which manages the connection with the Twitter API
     */
    public class TwitterConnection
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly TwitterAuthorizeRequest authorizeRequest = new TwitterAuthorizeRequest();

        // Gets the HTTPClientFactory through dependency injection
        public TwitterConnection(IHttpClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        // Method which gets the feed from Twitter
        public string getFeed()
        {
            return sendRequest().Result.ToString();
        }

        // Method which issues a HTTP request to the Twitter API for getting the Twitter feed
        public async Task<HttpResponseMessage> sendRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.twitter.com/1.1/statuses/home_timeline.json");
            authorizeRequest.signHttpRequest(request);
            var client = clientFactory.CreateClient();
            var response = await client.SendAsync(request);
            return response;
        }
    }
}
