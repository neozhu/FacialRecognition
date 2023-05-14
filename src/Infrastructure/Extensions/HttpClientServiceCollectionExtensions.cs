using System.Net.Http;
using System.Net.Http.Headers;
using Polly;

namespace CleanArchitecture.Blazor.Infrastructure.Extensions;
public static class HttpClientServiceCollectionExtensions
{
    public static void AddHttpClientService(this IServiceCollection services)
    {
        services.AddHttpClient("ocr", c =>
        {
            c.BaseAddress = new Uri("https://paddleocr.i247365.net/predict/ocr_system");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(30)));
        services.AddHttpClient("Insightface", c =>
        {
            c.BaseAddress = new Uri("https://face.tuutoo.top:11443");
            c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            c.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "multipart/form-data");
        }).AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(3)));
    }
}
