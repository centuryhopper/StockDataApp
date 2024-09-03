

using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Client.Interfaces;
using Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Shared.Models;
using static Shared.Models.ServiceResponses;

namespace Client.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient httpClient;
    private readonly ILocalStorageService localStorageService;
    private readonly AuthenticationStateProvider authenticationStateProvider;

    public AccountService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
    {
        httpClient = httpClientFactory.CreateClient("API");
        this.localStorageService = localStorageService;
        this.authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task<LoginResponse> LoginAsync(LoginDTO loginDTO)
    {
        var response = await httpClient.PostAsJsonAsync("api/Account/login", loginDTO);
        var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to validate session for user");
        }
        if (string.IsNullOrWhiteSpace(loginResponse.Token))
        {
            throw new Exception("Couldn't get a token");
        }
        await localStorageService.SetItemAsync("authToken", loginResponse!.Token);
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsAuthenticated(loginResponse!.Token);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", loginResponse!.Token);

        return loginResponse!;
    }

    public async Task LogoutAsync()
    {
        await localStorageService.RemoveItemAsync("authToken");
        ((ApiAuthenticationStateProvider)authenticationStateProvider).MarkUserAsLoggedOut();
        httpClient.DefaultRequestHeaders.Authorization = null;
    }
}