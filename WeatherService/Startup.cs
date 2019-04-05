using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;

namespace WeatherService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IAsyncPolicy<HttpResponseMessage> retryPolicy =
                Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                    .RetryAsync(3, onRetry: (response, retryCount) =>
                    {
                        Console.WriteLine($"Retrying {retryCount}");
                    });

            IAsyncPolicy<HttpResponseMessage> circuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .AdvancedCircuitBreakerAsync(0.5, TimeSpan.FromSeconds(60), 7, TimeSpan.FromSeconds(10),
                    OnBreak, OnReset, OnHalfOpen);

            IAsyncPolicy<HttpResponseMessage> circuitBreakerWrappedInRetryPolicy =
                Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(circuitBreakerWrappedInRetryPolicy);

            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:6001/") // this is the address of the temperature service
            };
            services.AddSingleton<HttpClient>(httpClient);

            services.AddMvc();
        }

        private void OnHalfOpen()
        {
            Console.WriteLine("Connection half open");
        }

        private void OnReset(Context context)
        {
            Console.WriteLine("Connection reset");
        }

        private void OnBreak(DelegateResult<HttpResponseMessage> delegateResult, TimeSpan timeSpan, Context context)
        {
            Console.WriteLine($"Connection break: {delegateResult.Result}, {delegateResult.Result}");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        } 
    }
}
