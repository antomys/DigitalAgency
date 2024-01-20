using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DigitalAgency.Blazor.Front.Infrastructure;
using DigitalAgency.Blazor.Front.LoginService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DigitalAgency.Blazor.Front;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddScoped<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
        services.AddScoped<ILocalStorageService, LocalStorageService>();
        services.AddServerSideBlazor();

        services.AddHttpClient();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see http://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();


        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();

            endpoints.MapFallbackToPage("/_Host");
        });
    }
}

public class TokenAuthenticationStateProvider : AuthenticationStateProvider
{
    readonly ILocalStorageService _localStorageService;

    public TokenAuthenticationStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        AuthenticationState ReturnAnonymous()
        {
            ClaimsIdentity anonymousIdentity = new ClaimsIdentity();
            ClaimsPrincipal anonPrincipals = new ClaimsPrincipal(anonymousIdentity);
            return new AuthenticationState(anonPrincipals);
        }

        SecurityToken token = await _localStorageService.GetAsync<SecurityToken>(nameof(SecurityToken));

        if (token == null)
        {
            return ReturnAnonymous();
        }

        if (string.IsNullOrEmpty(token.AccessToken))
        {
            return ReturnAnonymous();
        }

        List<Claim> claims = new List<Claim> {
            new(ClaimTypes.Name, token.UserName), new(ClaimTypes.Expired, token.ExpiredAt.ToLongDateString()),
        };
        ClaimsIdentity identity = new ClaimsIdentity(claims, "Token");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);
        return new AuthenticationState(principal);
    }
}