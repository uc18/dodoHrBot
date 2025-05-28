using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Interfaces;
using DodoBot.Models.Huntflow.Response;
using DodoBot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DodoBot.Services;

public class HuntflowService : IHuntflowService
{
    private readonly IOptions<ApplicationOptions> _options;

    private readonly HttpClient _httpClient;

    private readonly ILogger<HuntflowService> _logger;

    private readonly IAuthenticateService _authenticateService;

    public HuntflowService(IOptions<ApplicationOptions> options, HttpClient httpClient,
        ILogger<HuntflowService> logger, IAuthenticateService authenticateService)
    {
        _options = options;
        _httpClient = httpClient;
        _logger = logger;
        _authenticateService = authenticateService;
    }

    public async Task<DictionaryResponse?> GetDodoStreamAsync()
    {
        return await SendMessage($"{_options.Value.HuntflowApiUrl}{UriApiConstants.Speciality}");
    }

    public async Task<DictionaryResponse?> GetDodoSubSpecialtyAsync()
    {
        return await SendMessage($"{_options.Value.HuntflowApiUrl}{UriApiConstants.SubSpeciality}");
    }

    public async Task<VacancyResponse?> GetVacanciesAsync(int page)
    {
        _logger.LogInformation("GetVacanciesAsync");

        var uri = $"{_options.Value.HuntflowApiUrl}{UriApiConstants.Vacancies}?state=OPEN&page={page}";
        var counter = 0;
        do
        {
            var token = await _authenticateService.GetRefreshToken();
            var requestMessage = PrepareRequestMessage(uri, token);
            var response = await _httpClient.SendAsync(requestMessage);
            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<VacancyResponse>(response.Content.ReadAsStream());
            }

            counter++;
        } while (counter < 3);

        return null;
    }

    public async Task<DictionaryResponse?> GetWorkFormatAsync()
    {
        return await SendMessage($"{_options.Value.HuntflowApiUrl}{UriApiConstants.WorkFormat}");
    }

    public async Task<DictionaryResponse?> GetCityResponseAsync()
    {
        return await SendMessage($"{_options.Value.HuntflowApiUrl}{UriApiConstants.City}");
    }

    public async Task<DictionaryResponse?> GetGradeResponseAsync()
    {
        return await SendMessage($"{_options.Value.HuntflowApiUrl}{UriApiConstants.Grade}");
    }

    private async Task<DictionaryResponse?> SendMessage(string uri)
    {
        _logger.LogInformation("Sending message to Huntflow API");
        var counter = 0;
        try
        {
            do
            {
                var token = await _authenticateService.GetRefreshToken();
                var requestMessage = PrepareRequestMessage(uri, token);
                var response = await _httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<DictionaryResponse>(response.Content.ReadAsStream());
                }

                counter++;
            } while (counter < 3);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error sending message to Huntflow API");
            throw;
        }

        return null;
    }

    private HttpRequestMessage PrepareRequestMessage(string uri, string accessToken)
    {
        var req = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(uri)
        };

        req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        return req;
    }
}