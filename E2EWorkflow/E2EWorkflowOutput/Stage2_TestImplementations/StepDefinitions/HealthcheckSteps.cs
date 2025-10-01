using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using NUnit.Framework;
using System.Net.Http;
using Newtonsoft.Json;

namespace TestProject.StepDefinitions
{
    [Binding]
    public class HealthcheckSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
        private readonly HttpClient _httpClient;

        public HealthcheckSteps(ScenarioContext scenarioContext, TestContext testContext)
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

        [And(@"I am authenticated with a valid token")]
        public async Task I_am_authenticated_with_a_valid_token()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I am authenticated with a valid token");
        }

        [When(@"I send a GET request to \"/healthcheck\"")]
        public async Task I_send_a_GET_request_to_healthcheck()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a GET request to "/healthcheck"");
        }

        [Then(@"I should receive a successful response")]
        public async Task I_should_receive_a_successful_response()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I should receive a successful response");
        }

        [And(@"the response should have the correct content type")]
        public async Task the_response_should_have_the_correct_content_type()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the response should have the correct content type");
        }

        [And(@"the response should contain the requested data")]
        public async Task the_response_should_contain_the_requested_data()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the response should contain the requested data");
        }

        [When(@"I send a request with an invalid resource ID")]
        public async Task I_send_a_request_with_an_invalid_resource_ID()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a request with an invalid resource ID");
        }

        [Then(@"I should receive a 404 Not Found response")]
        public async Task I_should_receive_a_404_Not_Found_response()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I should receive a 404 Not Found response");
        }

        [Given(@"I have an invalid authentication token")]
        public async Task I_have_an_invalid_authentication_token()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I have an invalid authentication token");
        }

        [Then(@"I should receive a 401 Unauthorized response")]
        public async Task I_should_receive_a_401_Unauthorized_response()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I should receive a 401 Unauthorized response");
        }

    }
}
