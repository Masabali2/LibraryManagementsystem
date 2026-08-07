using System.Net.Http.Json;
using Library.Domain.DTOs.AI;

namespace Library.Infrastructure.Services;

public class RecommendationApiService
{
    private readonly HttpClient _httpClient;

    public RecommendationApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<BookRecommendationDto>> GetRecommendationsAsync(int studentId)
    {
        try
        {
            var response =
                await _httpClient.GetFromJsonAsync<RecommendationResponseDto>(
                    $"recommend/{studentId}");

            return response?.Recommendations ?? new();
        }
        catch
        {
            return new List<BookRecommendationDto>();
        }
    }
}