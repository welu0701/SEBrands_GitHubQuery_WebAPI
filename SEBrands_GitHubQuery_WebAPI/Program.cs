using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SEBrands_GitHubQuery_WebAPI.DTOs;
using System.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
//builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/getTopFiveStarredRepos", async ([FromQuery] string language, [FromServices] IHttpClientFactory httpClientFactory, [FromServices] IConfiguration configuration) =>
{
    try
    {
        var url = configuration.GetValue<string>("GitHubSerchURL");

        if (String.IsNullOrEmpty(url)) return Results.Problem("URL to GitHub API not defined in app settings");

        url += $"language:{HttpUtility.UrlEncode(language)}";

        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.UserAgent.TryParseAdd("request");

        var responseMessage = await client.GetAsync(url);
        responseMessage.EnsureSuccessStatusCode();
        string responseBody = await responseMessage.Content.ReadAsStringAsync();
        var responseDto = JsonConvert.DeserializeObject<ResponseDto>(responseBody);

        var outputDto = new OutputDto()
        {
            Message = responseDto.incomplete_results ? "Warning: Results may be incomplete" : "Success",
            RepoNames = responseDto?.items?.Select(x => x.name).ToList<string>()
        };

        return Results.Ok(outputDto);    
    }
    catch (global::System.Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }

})
.WithName("GetTopFiveStarredRepos");

app.Run();

