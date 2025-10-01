using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using NUnit.Framework;
using System.Net.Http;
using Newtonsoft.Json;

namespace TestProject.StepDefinitions
{
    [Binding]
    public class Health_ChecksSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
        private readonly HttpClient _httpClient;

        public Health_ChecksSteps(ScenarioContext scenarioContext, TestContext testContext)
        {
            _scenarioContext = scenarioContext;
            _testContext = testContext;
            _httpClient = testContext.HttpClient;
        }

        [Given(@"the API base URL is configured")]
        public async Task the_API_base_URL_is_configured()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the API base URL is configured");
        }

        [When(@"I send a health check request")]
        public async Task I_send_a_health_check_request()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a health check request");
        }

        [Then(@"I should receive a 200 OK response")]
        public async Task I_should_receive_a_200_OK_response()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I should receive a 200 OK response");
        }

        [And(@"the response should contain health status information")]
        public async Task the_response_should_contain_health_status_information()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the response should contain health status information");
        }

    }
}
