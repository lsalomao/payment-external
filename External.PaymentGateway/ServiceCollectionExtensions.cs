using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace External.PaymentGateway
{
    public static class ServiceCollectionExtensions
    {

        public static  IServiceCollection AddTransientWithRetry<TService, TException>(
                this IServiceCollection services, 
                Func<IServiceProvider, TService> implementationFactory,
                int retryCount = 3
            )
        where TService : class
        where TException: Exception
        {
            services.AddTransient<TService>(sp => {

                TService service = default(TService);

                var policy = Policy
                  .Handle<TException>()
                  .WaitAndRetry(retryCount, retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                  );

                policy.Execute(() =>
                {
                    service = implementationFactory(sp);
                });

                return service;

            });

            return services;
        }

        

    }
}
