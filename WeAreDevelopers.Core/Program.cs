// ---------------------------------------------------------------
// Copyright (c) Mabrouk Mahdhi. 
//  W/ love for WeAreDevelopers World Congress 2024.
// ---------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Standard.AI.OpenAI.Clients.OpenAIs;
using Standard.AI.OpenAI.Models.Configurations;
using WeAreDevelopers.Core.Brokers.DateTimes;
using WeAreDevelopers.Core.Brokers.Loggings;
using WeAreDevelopers.Core.Brokers.OpenAis;
using WeAreDevelopers.Core.Brokers.Storages;
using WeAreDevelopers.Core.Services.Foundations.Attendees;
using WeAreDevelopers.Core.Services.Foundations.OpenAis;

namespace WeAreDevelopers.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var openAIConfigurations = new OpenAIConfigurations
            {
                ApiKey = Environment.GetEnvironmentVariable("OpenAiKey"),
                OrganizationId = Environment.GetEnvironmentVariable("OpenAiOrgKey")
            };

            builder.Services.AddSingleton<IOpenAIClient>(client =>
                new OpenAIClient(openAIConfigurations));

            builder.Services.AddScoped<IDateTimeBroker, DateTimeBroker>();
            builder.Services.AddScoped<ILoggingBroker, LoggingBroker>();
            builder.Services.AddScoped<IStorageBroker, StorageBroker>();
            builder.Services.AddScoped<IOpenAiBroker, OpenAiBroker>();

            builder.Services.AddScoped<IAttendeeService, AttendeeService>();
            builder.Services.AddScoped<IOpenAiService, OpenAiService>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
