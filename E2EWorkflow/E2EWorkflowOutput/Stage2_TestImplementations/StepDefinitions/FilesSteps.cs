using System;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using NUnit.Framework;
using System.Net.Http;
using Newtonsoft.Json;

namespace TestProject.StepDefinitions
{
    [Binding]
    public class FilesSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly TestContext _testContext;
        private readonly HttpClient _httpClient;

        public FilesSteps(ScenarioContext scenarioContext, TestContext testContext)
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

        [And(@"I have valid request data")]
        public async Task I_have_valid_request_data()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I have valid request data");
        }

        [When(@"I send a POST request to \"/api/v1/files\"")]
        public async Task I_send_a_POST_request_to_apiv1files()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a POST request to "/api/v1/files"");
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

        [And(@"the response should include the created resource")]
        public async Task the_response_should_include_the_created_resource()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the response should include the created resource");
        }

        [And(@"the resource should be persisted")]
        public async Task the_resource_should_be_persisted()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the resource should be persisted");
        }

        [And(@"I have invalid request data")]
        public async Task I_have_invalid_request_data()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I have invalid request data");
        }

        [Then(@"I should receive a 400 Bad Request response")]
        public async Task I_should_receive_a_400_Bad_Request_response()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I should receive a 400 Bad Request response");
        }

        [And(@"the response should include validation errors")]
        public async Task the_response_should_include_validation_errors()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: the response should include validation errors");
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

        [When(@"I send a GET request to \"/api/v1/files/{attachmentId}\"")]
        public async Task I_send_a_GET_request_to_apiv1filesattachmentId()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a GET request to "/api/v1/files/{attachmentId}"");
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

        [When(@"I send a DELETE request to \"/api/v1/files/{attachmentId}\"")]
        public async Task I_send_a_DELETE_request_to_apiv1filesattachmentId()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a DELETE request to "/api/v1/files/{attachmentId}"");
        }

        [And(@"I have valid partial update data")]
        public async Task I_have_valid_partial_update_data()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I have valid partial update data");
        }

        [When(@"I send a PATCH request to \"/api/v1/files/{attachmentId}\"")]
        public async Task I_send_a_PATCH_request_to_apiv1filesattachmentId()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a PATCH request to "/api/v1/files/{attachmentId}"");
        }

        [When(@"I send a GET request to \"/api/v1/files/{key}/{keyValue}\"")]
        public async Task I_send_a_GET_request_to_apiv1fileskeykeyValue()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a GET request to "/api/v1/files/{key}/{keyValue}"");
        }

        [When(@"I send a GET request to \"/api/v1/files/{attachmentId}/metadata\"")]
        public async Task I_send_a_GET_request_to_apiv1filesattachmentIdmetadata()
        {
            // TODO: Implement step
            Console.WriteLine("Executing: I send a GET request to "/api/v1/files/{attachmentId}/metadata"");
        }

    }
}
