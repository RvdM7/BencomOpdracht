using BencomOpdracht.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;

namespace BencomOpdracht.Controllers
{
    // Controller for the homepage
    public class HomeController : Controller
    {
        private TwitterConnection twitterConnection;

        // Gets the HTTPClientFactory through dependency injection and passes this to the TwitterConnection class
        public HomeController(IHttpClientFactory clientFactory)
        {
            twitterConnection = new TwitterConnection(clientFactory);
        }

        public String Index()
        {
            return twitterConnection.getFeed();
        }
    }
}
