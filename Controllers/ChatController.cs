using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ChatGptProxyApi.Models;

namespace ChatGptProxyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ChatRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("OpenAI");

            var requestBody = new
            {
                model = "text-davinci-003", // or the latest model you have access to
                prompt = request.Prompt,
                max_tokens = 150
            };

            var response = await httpClient.PostAsJsonAsync("chat/completions", requestBody);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<object>(responseContent);
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to get response from OpenAI");
            }
        }
    }
}
