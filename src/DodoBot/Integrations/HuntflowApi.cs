using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Integrations;
using DodoBot.Interfaces.Services;
using DodoBot.Models.Huntflow.Response;
using DodoBot.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DodoBot.Integrations;

public class HuntflowApi(
    IOptions<ApplicationOptions> options,
    HttpClient httpClient,
    ILogger<HuntflowApi> logger,
    IAuthenticateService authenticateService)
    : IHuntflowApi
{
    public async Task<DictionaryResponse?> GetDodoStreamAsync()
    {
        return await SendMessage($"{options.Value.HuntflowApiUrl}{UriApiConstants.Speciality}");
    }

    public async Task<DictionaryResponse?> GetDodoSubSpecialtyAsync()
    {
        return await SendMessage($"{options.Value.HuntflowApiUrl}{UriApiConstants.SubSpeciality}");
    }

    public async Task<VacancyResponse?> GetVacanciesAsync(int page)
    {
        logger.LogInformation("GetVacanciesAsync");

        var uri = $"{options.Value.HuntflowApiUrl}{UriApiConstants.Vacancies}?state=OPEN&page={page}";
        var counter = 0;
        var isTokenValid = true;
        do
        {
            var token = await GetToken(isTokenValid);

            if (token.Length > 0)
            {
                var requestMessage = PrepareRequestMessage(uri, token);
                var response = await httpClient.SendAsync(requestMessage);
                if (response.IsSuccessStatusCode)
                {
                    return await JsonSerializer.DeserializeAsync<VacancyResponse>(response.Content.ReadAsStream());
                }
            }

            isTokenValid = false;

            counter++;
        } while (counter < 3);

        return null;
    }

    private async Task<DictionaryResponse?> SendMessage(string uri)
    {
        logger.LogInformation("Sending message to Huntflow API");
        var counter = 0;
        var isTokenValid = true;
        try
        {
            do
            {
                var token = await GetToken(isTokenValid);
                if (token.Length > 0)
                {
                    var requestMessage = PrepareRequestMessage(uri, token);
                    var response = await httpClient.SendAsync(requestMessage);
                    if (response.IsSuccessStatusCode)
                    {
                        return await JsonSerializer.DeserializeAsync<DictionaryResponse>(
                            response.Content.ReadAsStream());
                    }
                }

                isTokenValid = false;

                counter++;
            } while (counter < 3);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error sending message to Huntflow API");
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

    private async Task<string> GetToken(bool isTokenValid)
    {
        return isTokenValid
            ? authenticateService.GetExistsAccessToken()
            : await authenticateService.GetNewAccessToken();
    }
}