using Api.Authentication;
using Api.Endpoints;
using Api.Middleware;
using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddShamlAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("VueDevelopment", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseCors("VueDevelopment");
}

app.UseAuthentication();
app.UseAuthorization();

app.MapUserEndpoints();

app.MapCenterEndpoints();

app.MapCurrentUserEndpoints();
app.MapAssignmentEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();