using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BlockChain.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BlockChain.Infrastructure.Entities;
using BlockChain.Infrastructure.Mapper;
using BlockChain.Domain;
using BlockChain.Infrastructure.Repositories;
using BlockChain.Controllers;

namespace BlockChain
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
            services.AddDbContext<ApiContext>((builder) => builder.UseInMemoryDatabase("blockchain-store"));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddScoped<IBlockchainMapper, BlockchainMapper>();
            services.AddScoped<IBlockchainRepository, BlockchainRepository>();
            services.AddSingleton<Node>(new Node());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }
            
            app.UseMvc();
        }
    }
}
