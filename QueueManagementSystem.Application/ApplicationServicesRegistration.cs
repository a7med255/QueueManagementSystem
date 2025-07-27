using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueueManagementSystem.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            //config of mediatr
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //config of automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //config of fluent validation
         //   services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

           // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
