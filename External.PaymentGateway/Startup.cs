using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using External.PaymentGateway.Entities;
using External.PaymentGateway.Repository;
using External.PaymentGateway;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using Polly;
using RabbitMQ.Client.Exceptions;

namespace External.PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            InitMongoDependencies(services);
            InitRabbitMQ(services);

            services.AddSingleton<ApplicationInitializer>();

            services.AddTransient<ProcessService>();

            services.AddControllers();
        }

        private void InitRabbitMQ(IServiceCollection services)
        {
            services.AddTransient(sp => new RabbitMQ.Client.ConnectionFactory()
            {
                HostName = Configuration.GetSection("Connections:RabbitMQ:Server").Value,
                Port = Configuration.GetSection("Connections:RabbitMQ:Port").Get<int>(),
                //VirtualHost = Configuration.GetSection("Connections:RabbitMQ:VirtualHost").Value,
                UserName = Configuration.GetSection("Connections:RabbitMQ:UserName").Value,
                Password = Configuration.GetSection("Connections:RabbitMQ:Password").Value,
            });

            services.AddTransientWithRetry<IConnection, BrokerUnreachableException>((sp) => sp.GetRequiredService<ConnectionFactory>().CreateConnection(), 3);

            services.AddTransient(sp => sp.GetRequiredService<RabbitMQ.Client.IConnection>().CreateModel());

            services.AddTransient<IConsumer<PaymentOrder>>(sp=> 
                new ReliableQueueConsumer<PaymentOrder>(sp.GetRequiredService<IModel>(), 3)
            );

        }

        private void InitMongoDependencies(IServiceCollection services)
        {
            services.AddTransient<MongoClient>(sp =>
            {
                string serverName = Configuration.GetSection("Connections:Mongo:Server").Value;
                string port = Configuration.GetSection("Connections:Mongo:Port").Value;
                string username = Configuration.GetSection("Connections:Mongo:Username").Value;
                string password = Configuration.GetSection("Connections:Mongo:Password").Value;

                return new MongoClient($"mongodb://{username}:{password}@{serverName}:{port}");
            });

            services.AddTransient(sp =>
            {
                string databaseName = Configuration.GetSection("Connections:Mongo:DB").Value;
                return sp.GetRequiredService<MongoClient>().GetDatabase(databaseName);
            });

            services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<PaymentOrder>("PaymentOrder"));
            services.AddTransient(sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<Account>("Account"));

            services.AddTransient<IQueryRepository<PaymentOrder>, MongoDBQueryRepository<PaymentOrder>>();
            services.AddTransient<IQueryRepository<Account>, MongoDBQueryRepository<Account>>();

            services.AddTransient<IPersistenceRepository<PaymentOrder>, MongoDBPersistenceRepository<PaymentOrder>>();
            services.AddTransient<IPersistenceRepository<Account>, MongoDBPersistenceRepository<Account>>();

            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<INotificationService, NotificationService>();



        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

            app.ApplicationServices.GetRequiredService<ApplicationInitializer>().Initialize();

            app.ApplicationServices.GetRequiredService<ProcessService>().Consume();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


        }
    }
}
