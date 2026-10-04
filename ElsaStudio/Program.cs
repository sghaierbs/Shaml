using Elsa.Studio.Authentication.Abstractions.Models;
using Elsa.Studio.Authentication.ElsaIdentity.BlazorServer.Extensions;
using Elsa.Studio.Authentication.ElsaIdentity.HttpMessageHandlers;
using Elsa.Studio.Authentication.ElsaIdentity.UI.Extensions;
using Elsa.Studio.Authentication.UI.Extensions;
using Elsa.Studio.Authentication.UI.Options;
using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;

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
// Authentication: Elsa Identity
// --------------------------------------------------

builder.Services.AddStudioAuthenticationMode(options =>
    options.Provider = StudioAuthenticationProvider.ElsaIdentity);

builder.Services.AddElsaIdentity();
builder.Services.AddElsaIdentityUI();

builder.Services.AddAuthenticationUI(
    configuration.GetSection(LoginThemeOptions.SectionName));


// --------------------------------------------------
// Connection to Elsa Server
// --------------------------------------------------

var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options =>
        configuration.GetSection("Backend").Bind(options),

    ConfigureHttpClientBuilder = options =>
    {
        options.AuthenticationHandler =
            typeof(ElsaIdentityAuthenticatingApiHttpMessageHandler);
    }
};


// --------------------------------------------------
// Elsa Studio modules
// --------------------------------------------------

builder.Services.AddCore();

builder.Services.AddShell();

builder.Services.AddRemoteBackend(backendApiConfig);

builder.Services.AddDashboardModule(backendApiConfig);

builder.Services.AddWorkflowsModule();


// --------------------------------------------------
// Build application
// --------------------------------------------------

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();