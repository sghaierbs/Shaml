using Elsa.Studio.Authentication.Abstractions.Models;
using Elsa.Studio.Authentication.ElsaIdentity.BlazorServer.Extensions;
using Elsa.Studio.Authentication.ElsaIdentity.HttpMessageHandlers;
using Elsa.Studio.Authentication.ElsaIdentity.UI.Extensions;
using Elsa.Studio.Authentication.UI.Extensions;
using Elsa.Studio.Authentication.UI.Options;
using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using ElsaStudio.Authentication;
using ElsaStudio.Features;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// --------------------------------------------------
// Blazor Server
// --------------------------------------------------

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor(options =>
{
    options.RootComponents.RegisterCustomElsaStudioElements();
    options.RootComponents.MaxJSRootComponents = 1000;
});


// --------------------------------------------------
// Elsa Studio Authentication Infrastructure
// --------------------------------------------------

// Restore Elsa Identity for now because it registers authentication
// infrastructure required by Elsa Studio, including services used by
// WorkflowInstanceObserverFactory.
builder.Services.AddStudioAuthenticationMode(options =>
    options.Provider = StudioAuthenticationProvider.ElsaIdentity);

builder.Services.AddElsaIdentity();
builder.Services.AddElsaIdentityUI();

builder.Services.AddAuthenticationUI(
    configuration.GetSection(LoginThemeOptions.SectionName));


// --------------------------------------------------
// Shaml JWT Support
// --------------------------------------------------

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<
    IShamlTokenAccessor,
    ShamlTokenAccessor>();

builder.Services.AddTransient<
    ShamlAuthenticatingApiHttpMessageHandler>();


// --------------------------------------------------
// Connection to Elsa Server
// --------------------------------------------------

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options =>
        configuration.GetSection("Backend").Bind(options),

    ConfigureHttpClientBuilder = options =>
    {
        // Temporarily restore Elsa Identity here as well.
        //
        // This gives us a known-good Studio baseline before replacing
        // the Studio authentication provider with Shaml authentication.
        options.AuthenticationHandler =
            typeof(ElsaIdentityAuthenticatingApiHttpMessageHandler);
    }
};


// --------------------------------------------------
// Elsa Studio Modules
// --------------------------------------------------

builder.Services.AddCore();

builder.Services.AddShell();

builder.Services.AddRemoteBackend(backendApiConfig);

builder.Services.AddDashboardModule(backendApiConfig);

builder.Services.AddWorkflowsModule();

builder.Services.AddScoped<IFeature, ShamlStudioFeature>();

// --------------------------------------------------
// Build Application
// --------------------------------------------------

var app = builder.Build();


// --------------------------------------------------
// HTTP Pipeline
// --------------------------------------------------

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapBlazorHub();

app.MapFallbackToPage("/_Host");

app.Run();