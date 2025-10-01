using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace TestProject
{
    public class TestContext
    {
        public HttpClient HttpClient { get; }
        public string BaseUrl { get; set; }
        public string AuthToken { get; set; }

        public TestContext()
        {
            HttpClient = new HttpClient();
            BaseUrl = "https://api.example.com";
        }

        public void SetAuthToken(string token)
        {
            AuthToken = token;
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
