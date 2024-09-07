using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
// using Blazored.Modal;
using Microsoft.AspNetCore.Components.Authorization;
using Client.Providers;
using Blazored.LocalStorage;
using Client.Interfaces;
using Client.Services;
using Client.Handlers;
using Radzen;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<AuthorizationMessageHandler>();

builder.Services
.AddHttpClient(
    "API",
    client => {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    }
)
// need this line for being able to use httpcontext.user.claims in the server controllers
.AddHttpMessageHandler<AuthorizationMessageHandler>();


builder.Services.AddScoped(
    sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("API")
);

builder.Services.AddBlazoredLocalStorageAsSingleton();


builder.Services.AddBlazorBootstrap();
// builder.Services.AddBlazoredModal();
builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IStockDataService, StockDataService>();


await builder.Build().RunAsync();
