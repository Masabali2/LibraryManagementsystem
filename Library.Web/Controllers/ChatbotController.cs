using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Library.Web.Controllers;

public class ChatbotController : Controller
{
    private readonly HttpClient _httpClient;

    public ChatbotController(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("LibraryChatbot");
    }

    // =========================================================
    // SEND MESSAGE TO FASTAPI
    // =========================================================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendMessage(
        [FromBody] ChatbotRequest request)
    {
        if (request == null ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                success = false,
                message = "Message cannot be empty."
            });
        }

        try
        {
            var payload = new
            {
                message = request.Message.Trim(),

                // Conversation ID allows FastAPI to maintain context
                conversation_id = request.ConversationId,

                // Optional student information
                student_name = HttpContext.Session
                    .GetString("StudentName") ?? "Student"
            };

            var json = JsonSerializer.Serialize(payload);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(
                "/chat",
                content
            );

            var responseContent =
                await response.Content.ReadAsStringAsync();

            // FastAPI returned an error
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    $"FastAPI Error: {response.StatusCode}"
                );

                Console.WriteLine(
                    $"FastAPI Response: {responseContent}"
                );

                return StatusCode(
                    (int)response.StatusCode,
                    new
                    {
                        success = false,
                        message =
                            "Library AI service returned an error."
                    }
                );
            }

            var chatbotResponse =
                JsonSerializer.Deserialize<ChatbotApiResponse>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }
                );

            if (chatbotResponse == null)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message =
                        "Invalid response received from Library AI."
                });
            }

            return Json(new
            {
                success = chatbotResponse.Success,

                response =
                    !string.IsNullOrWhiteSpace(
                        chatbotResponse.Response)
                        ? chatbotResponse.Response
                        : chatbotResponse.Message
            });
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine(
                $"FastAPI connection error: {ex.Message}"
            );

            return StatusCode(503, new
            {
                success = false,
                message =
                    "Library AI is currently unavailable. " +
                    "Please make sure the FastAPI chatbot is running."
            });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, new
            {
                success = false,
                message =
                    "Library AI took too long to respond. " +
                    "Please try again."
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Chatbot error: {ex}"
            );

            return StatusCode(500, new
            {
                success = false,
                message =
                    "An unexpected error occurred while " +
                    "processing your message."
            });
        }
    }


    // =========================================================
    // HEALTH CHECK
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> Health()
    {
        try
        {
            var response =
                await _httpClient.GetAsync("/health");

            var content =
                await response.Content.ReadAsStringAsync();

            return Content(
                content,
                "application/json"
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Chatbot health check error: {ex.Message}"
            );

            return StatusCode(503, new
            {
                success = false,
                status = "offline"
            });
        }
    }
}


// =============================================================
// REQUEST MODEL
// =============================================================

public class ChatbotRequest
{
    public string Message { get; set; } = string.Empty;

    public string? ConversationId { get; set; }
}


// =============================================================
// FASTAPI RESPONSE MODEL
// =============================================================

public class ChatbotApiResponse
{
    public bool Success { get; set; }

    public string? Response { get; set; }

    // Also support FastAPI returning "message"
    public string? Message { get; set; }
}