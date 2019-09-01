using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace BencomOpdracht.Models
{
    /*
     * Class for creating a signature
     * A signature is the body, URL, Authorization header and the HTTP method put into an algorithm
     * This creates a code, which is put into the Authorization header. Twitter can check this on their side to check if the request has been tampered with
     */
    public class TwitterCreateSignature
    {
        // Class for storing the various keys and codes Twitter provides, which are needed to communicate with their API
        public class Keys
        {
            // The "secret" keys are never send but only used when creating a signature
            public readonly string apiKey;
            public readonly string apiKeySecret;
            public readonly string accessToken;
            public readonly string accessTokenSecret;

            public Keys(string apiKey, string apiKeySecret, string accessToken, string accessTokenSecret)
            {
                this.apiKey = apiKey;
                this.apiKeySecret = apiKeySecret;
                this.accessToken = accessToken;
                this.accessTokenSecret = accessTokenSecret;
            }
        }
        private readonly Keys keys;

        public TwitterCreateSignature(Keys keys)
        {
            this.keys = keys;
        }

        /*
         * Method for creating the base string used in creating the signature
         * First the signature header is put into a string and percent encoded (special characters replaced by "%XX" where XX is a number indicating the original character)
         * Then the HTTP method and URL are prepended into that string (without percent encoding)
         */
        private byte[] generateSignatureBaseString(HttpRequestMessage httpRequest, Dictionary<string, string> authHeaderValues)
        {
            SortedDictionary<string, string> keyValues = new SortedDictionary<string, string>(authHeaderValues);
            StringBuilder stringBuilder = new StringBuilder();

            //TODO add HTTP body processing

            foreach (KeyValuePair<string, string> entry in keyValues)
            {
                if(entry.Key == "signature")
                {
                    continue;
                }
                stringBuilder
                    .Append("oauth_")
                    .Append(entry.Key)
                    .Append("=")
                    .Append(entry.Value)
                    .Append("&");
            }

            stringBuilder.Length--;
            var authHeaderString = Uri.EscapeDataString(stringBuilder.ToString());
            stringBuilder.Length = 0;

            stringBuilder
            .Append(httpRequest.Method.ToString().ToUpper())
            .Append("&")
            .Append(Uri.EscapeDataString(httpRequest.RequestUri.ToString()))
            .Append("&")
            .Append(authHeaderString);
            return Encoding.ASCII.GetBytes(stringBuilder.ToString());
        }

        // Method to get the Key used in generating the signature
        private byte[] generateSignatureKey()
        {
            return Encoding.ASCII.GetBytes(keys.apiKeySecret + "&" + keys.accessTokenSecret);
        }

        /*
         * From:
         * https://stackoverflow.com/questions/6067751/how-to-generate-hmac-sha1-in-c
         * Using the bytes of the base string and key, a signature is created and converted to base64
         */
        public string createHMACSHA1Signature(HttpRequestMessage httpRequest, Dictionary<string, string> authHeaderValues)
        {
            HMACSHA1 signature = new HMACSHA1(generateSignatureKey());
            MemoryStream stream = new MemoryStream(generateSignatureBaseString(httpRequest, authHeaderValues));
            var hash = signature.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
            var bytes = Encoding.ASCII.GetBytes(hash);
            return Convert.ToBase64String(bytes);
        }
    }
}
