using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

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
                    .RetryAsync(3, (response, retryCount) =>
                    {
                        if (response.Result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            Console.WriteLine("Retrying is not going to help here! I wish I had .NET Core 2.1 and HttpClientFactory.");
                        }
                        else
                        {
                            Console.WriteLine("Do something else.");
                        }
                    });

            services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(retryPolicy);

            HttpClient httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://localhost:6001/") // this is the address of the temperature service
            };
            services.AddSingleton<HttpClient>(httpClient);

            services.AddMvc();
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
