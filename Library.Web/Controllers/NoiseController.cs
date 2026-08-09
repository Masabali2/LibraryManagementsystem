using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Library.Web.Controllers;

    public class NoiseController : Controller
    {
        private readonly HttpClient _httpClient;

        public NoiseController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("NoiseApi");
        }


        // ============================================================
        // GET ALL NOISE ALERTS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Alerts()
        {
            try
            {
                var response = await _httpClient.GetAsync("/noise/alerts");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Noise API is unavailable."
                        }
                    );
                }

                var data =
                    await response.Content.ReadFromJsonAsync<NoiseAlertsResponse>();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Unable to connect to Noise Detection API.",
                        error = ex.Message
                    }
                );
            }
        }


        // ============================================================
        // GET UNREAD ALERTS
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Unread()
        {
            try
            {
                var response =
                    await _httpClient.GetAsync("/noise/alerts/unread");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Noise API is unavailable."
                        }
                    );
                }

                var data =
                    await response.Content.ReadFromJsonAsync<NoiseAlertsResponse>();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Unable to connect to Noise Detection API.",
                        error = ex.Message
                    }
                );
            }
        }


        // ============================================================
        // GET UNREAD COUNT
        // ============================================================

        [HttpGet]
        public async Task<IActionResult> Count()
        {
            try
            {
                var response =
                    await _httpClient.GetAsync("/noise/alerts/count");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Noise API is unavailable."
                        }
                    );
                }

                var data =
                    await response.Content.ReadFromJsonAsync<NoiseCountResponse>();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Unable to connect to Noise Detection API.",
                        error = ex.Message
                    }
                );
            }
        }


        // ============================================================
        // MARK SINGLE ALERT AS READ
        // ============================================================

        [HttpPut]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                var response =
                    await _httpClient.PutAsync(
                        $"/noise/alerts/{id}/read",
                        null
                    );

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Unable to mark alert as read."
                        }
                    );
                }

                var data =
                    await response.Content.ReadFromJsonAsync<NoiseActionResponse>();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Noise API connection failed.",
                        error = ex.Message
                    }
                );
            }
        }


        // ============================================================
        // MARK ALL ALERTS AS READ
        // ============================================================

        [HttpPut]
        public async Task<IActionResult> MarkAllAsRead()
        {
            try
            {
                var response =
                    await _httpClient.PutAsync(
                        "/noise/alerts/read-all",
                        null
                    );

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Unable to mark alerts as read."
                        }
                    );
                }

                var data =
                    await response.Content.ReadFromJsonAsync<NoiseActionResponse>();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Noise API connection failed.",
                        error = ex.Message
                    }
                );
            }
        }


        // ============================================================
        // CLEAR ALL ALERTS
        // ============================================================

        [HttpDelete]
        public async Task<IActionResult> ClearAll()
        {
            try
            {
                var response =
                    await _httpClient.DeleteAsync("/noise/alerts");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Unable to clear noise alerts."
                        }
                    );
                }

                var data =
                    await response.Content.ReadFromJsonAsync<NoiseActionResponse>();

                return Json(data);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    new
                    {
                        success = false,
                        message = "Noise API connection failed.",
                        error = ex.Message
                    }
                );
            }
        }
    }


    // ============================================================
    // DTOs
    // ============================================================

    public class NoiseAlert
    {
        public int Id { get; set; }

        public string Location { get; set; } = "";

        public int Desk_Number { get; set; }

        public int Chair_Number { get; set; }

        public string Message { get; set; } = "";

        public DateTime Detected_At { get; set; }

        public bool Is_Read { get; set; }
    }


    public class NoiseAlertsResponse
    {
        public bool Success { get; set; }

        public int Count { get; set; }

        public List<NoiseAlert> Alerts { get; set; } = new();
    }


    public class NoiseCountResponse
    {
        public bool Success { get; set; }

        public int Count { get; set; }
    }


    public class NoiseActionResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; } = "";
    }
