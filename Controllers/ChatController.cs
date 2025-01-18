using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ChatGptProxyApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace ChatGptProxyApi.Controllers
{
  [Authorize]
  [ApiController]
  [Route("api/chat")]
  public class ChatController : ControllerBase
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public ChatController(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory;
    }

    [HttpPost]
    public async Task<IActionResult> PostMessage([FromBody] ChatRequest chatRequest)
    {
      var httpClient = _httpClientFactory.CreateClient("OpenAI");
      var response = await httpClient.PostAsJsonAsync("chat/completions", new
      {
        model = "text-davinci-003", // Specify the model
        prompt = chatRequest.Prompt,
        max_tokens = 150
      });

      if (response.IsSuccessStatusCode)
      {
        var responseContent = await response.Content.ReadAsStringAsync();
        return Ok(responseContent);
      }

      return StatusCode((int)response.StatusCode, response.ReasonPhrase);
    }
  }

}
