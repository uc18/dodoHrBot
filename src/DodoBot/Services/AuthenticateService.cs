using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Interfaces;
using DodoBot.Interfaces.Services;
using DodoBot.Models.Huntflow;
using DodoBot.Options;
using Microsoft.Extensions.Options;

namespace DodoBot.Services;

public class AuthenticateService(HttpClient httpClient, IOptions<ApplicationOptions> options)
    : IAuthenticateService
{
    private string AccessToken { get; set; } = options.Value.HuntflowTokens.HuntflowAccessTokenApi;

    private string RefreshToken { get; set; } = options.Value.HuntflowTokens.HuntflowRefreshTokenApi;

    private string HuntflowDodoBrandsApiUrl { get; } = options.Value.HuntflowApiUrl;

    private DateTime _expirationDate = DateTime.UtcNow.AddSeconds(options.Value.HuntflowTokens.HuntflowTokenLifeTime);

    public string GetExistsAccessToken()
    {
        return _expirationDate < DateTime.UtcNow ? string.Empty : AccessToken;
    }

    public async Task<string> GetNewAccessToken()
    {
        var content = JsonContent.Create(new TokenRequest
        {
            Token = RefreshToken
        });
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", AccessToken);

        var request = await httpClient.PostAsync(
            $"{HuntflowDodoBrandsApiUrl}{UriApiConstants.RefreshTokenUri}", content);

        if (request.IsSuccessStatusCode)
        {
            var response = await request.Content.ReadAsStreamAsync();

            var tokenResponse = await JsonSerializer.DeserializeAsync<TokenResponse>(response);

            if (tokenResponse != null)
            {
                AccessToken = tokenResponse.AccessToken;
                RefreshToken = tokenResponse.RefreshToken;
                _expirationDate = DateTime.UtcNow.AddSeconds(tokenResponse.AccessTokenExpiration);

                return AccessToken;
            }
        }

        return string.Empty;
    }
}