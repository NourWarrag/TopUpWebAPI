
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using TopUp.API.Data;
using TopUp.API.Infrastructure;
using TopUp.API.Services;
using TopUp.API.Services.UserBalance;

namespace TopUp.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<TopUpDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddHttpClient<IUserBalanceService, UserBalanceService>(options =>
            {
                options.BaseAddress = new Uri(builder.Configuration["UserBalanceAPI"]);
            }).AddPolicyHandler(GetRetryPolicy());

            builder.Services.AddScoped<ITopUpService, TopUpService>();
            builder.Services.AddControllers( options => options.Filters.Add<ApiExceptionFilter>());

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            await app.InitialiseDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.Run();
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {

            var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

            var retryPolicy = HttpPolicyExtensions.HandleTransientHttpError()
                 .WaitAndRetryAsync(delay);

            return retryPolicy;
        }
    }
}
