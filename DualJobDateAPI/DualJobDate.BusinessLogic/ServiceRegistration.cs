﻿using DualJobDate.BusinessLogic.Helper;
using DualJobDate.BusinessLogic.Services;
using DualJobDate.BusinessObjects.Entities.Interface.Helper;
using DualJobDate.BusinessObjects.Entities.Interface.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DualJobDate.BusinessLogic.Services.Interface;

namespace DualJobDate.BusinessLogic
{
    public static class ServiceRegistration
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<ITestService, TestService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailHelper, EmailHelper>();
            _ = services.AddTransient<IJwtHelper, JwtHelper>(x =>
            {
                var configuration = x.GetRequiredService<IConfiguration>();
                var secretKey = configuration["JwtSecret"] ?? throw new NotImplementedException("JwtSecret is not defined");
                return new JwtHelper(secretKey);
            });
            services.AddTransient<ICompanyService, CompanyService>();
        }

    }
}
