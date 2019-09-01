using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using static BencomOpdracht.Models.TwitterCreateSignature;

namespace BencomOpdracht.Models
{
    /*
     * Class for authorizing a HTTP request before sending this to Twitter
     * This class adds the Authorization header and handles its various components
     */
    public class TwitterAuthorizeRequest
    {
        private TwitterCreateSignature createSignature;
        private Dictionary<string, string> authHeaderValues = new Dictionary<string, string>();
        private static Random random = new Random();

        /*
         * Constructor loads API keys and passes them to TwitterCreateSignature
         * The authHeaderValues dictionary is filled with values which are necessary when sending a HTTP request to the Twitter API
         */
        public TwitterAuthorizeRequest()
        {
            // Get authorization keys
            Keys keys = JsonConvert.DeserializeObject<Keys>(File.ReadAllText(@"Keys.json"));
            createSignature = new TwitterCreateSignature(keys);

            // Add oauth header values
            authHeaderValues.Add("consumer_key", keys.apiKey);
            authHeaderValues.Add("nonce", null);
            authHeaderValues.Add("signature", null);
            authHeaderValues.Add("signature_method", "HMAC-SHA1");
            authHeaderValues.Add("timestamp", null);
            authHeaderValues.Add("token", keys.accessToken);
            authHeaderValues.Add("version", "1.0");
        }

        // Method which creates the Authorizations header and adds it to the request, also regenerates certain values
        public void signHttpRequest(HttpRequestMessage httpRequest)
        {
            fillRandoms(httpRequest);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("OAuth ");
            foreach(KeyValuePair<string, string> entry in authHeaderValues)
            {
                stringBuilder
                    .Append("oauth_")
                    .Append(Uri.EscapeDataString(entry.Key))
                    .Append("=\"")
                    .Append(Uri.EscapeDataString(entry.Value))
                    .Append("\", ");
            }
            stringBuilder.Length -= 2;
            httpRequest.Headers.Add("Authorization", stringBuilder.ToString());
        }

        /*
         * Method for regenerating various values used in the Authorization header
         * nonce: a random number which is unique among requests
         * timestamp: timestamp in unix time(seconds from some date in 1970)
         * signature: value which depicts all values from the HTTP body, URL, HTTP method and authorization header.
         */
        private void fillRandoms(HttpRequestMessage httpRequest)
        {
            authHeaderValues["nonce"] = getRandomNonce();
            authHeaderValues["timestamp"] = getCurrentTime();
            authHeaderValues["signature"] = Uri.EscapeDataString(createSignature.createHMACSHA1Signature(httpRequest, authHeaderValues));
        }
        
        // Method to get the current unix time
        private String getCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        }

        // Method to get a random alphanumerical string of length 42
        private String getRandomNonce()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[42];

            for(int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
