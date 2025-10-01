using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TestProject
{
    public class AttachmentServiceAPIClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public AttachmentServiceAPIClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        public async Task<HttpResponseMessage> POST_api_v1_files(object requestData = null)
        {
            var url = $"{_baseUrl}/api/v1/files";

            var content = requestData != null ?
                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json") :
                null;

            var request = new HttpRequestMessage(HttpMethod.POST, url)
            {
                Content = content
            };
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GET_api_v1_files_attachmentId(object requestData = null)
        {
            var url = $"{_baseUrl}/api/v1/files/{attachmentId}";

            var request = new HttpRequestMessage(HttpMethod.GET, url);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> DELETE_api_v1_files_attachmentId(object requestData = null)
        {
            var url = $"{_baseUrl}/api/v1/files/{attachmentId}";

            var request = new HttpRequestMessage(HttpMethod.DELETE, url);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> PATCH_api_v1_files_attachmentId(object requestData = null)
        {
            var url = $"{_baseUrl}/api/v1/files/{attachmentId}";

            var content = requestData != null ?
                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json") :
                null;

            var request = new HttpRequestMessage(HttpMethod.PATCH, url)
            {
                Content = content
            };
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GET_api_v1_files_key_keyValue(object requestData = null)
        {
            var url = $"{_baseUrl}/api/v1/files/{key}/{keyValue}";

            var request = new HttpRequestMessage(HttpMethod.GET, url);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GET_api_v1_files_attachmentId_metadata(object requestData = null)
        {
            var url = $"{_baseUrl}/api/v1/files/{attachmentId}/metadata";

            var request = new HttpRequestMessage(HttpMethod.GET, url);
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> POST_copy-file_attachmentId_newFileName(object requestData = null)
        {
            var url = $"{_baseUrl}/copy-file/{attachmentId}/{newFileName}";

            var content = requestData != null ?
                new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json") :
                null;

            var request = new HttpRequestMessage(HttpMethod.POST, url)
            {
                Content = content
            };
            return await _httpClient.SendAsync(request);
        }

        public async Task<HttpResponseMessage> GET_healthcheck(object requestData = null)
        {
            var url = $"{_baseUrl}/healthcheck";

            var request = new HttpRequestMessage(HttpMethod.GET, url);
            return await _httpClient.SendAsync(request);
        }

    }
}
