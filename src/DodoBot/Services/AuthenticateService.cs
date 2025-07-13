using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using DodoBot.Constants;
using DodoBot.Interfaces;
using DodoBot.Models.Huntflow;
using DodoBot.Options;
using Microsoft.Extensions.Options;

namespace DodoBot.Services;

public class AuthenticateService : IAuthenticateService
{
    private readonly HttpClient _httpClient;

    private string AccessToken { get; set; }

    private string RefreshToken { get; set; }

    private string HuntflowDodoBrandsApiUrl { get; }

    private DateTime _expirationDate;

    public AuthenticateService(HttpClient httpClient, IOptions<ApplicationOptions> options)
    {
        _httpClient = httpClient;
        _expirationDate = DateTime.UtcNow.AddSeconds(options.Value.HuntflowTokens.HuntflowTokenLifeTime);
        AccessToken = options.Value.HuntflowTokens.HuntflowAccessTokenApi;
        RefreshToken = options.Value.HuntflowTokens.HuntflowRefreshTokenApi;
        HuntflowDodoBrandsApiUrl = options.Value.HuntflowApiUrl;
    }

    public async Task<string> GetRefreshToken()
    {
        if (_expirationDate < DateTime.UtcNow)
        {
            var content = JsonContent.Create(new TokenRequest
            {
                Token = RefreshToken
            });
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", AccessToken);

            var request = await _httpClient.PostAsync(
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

        return AccessToken;
    }
}