using System.Reflection;
using BuildingBlocks.Behaviors;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.DependencyInjection;
using BuildingBlocks.Cache;
using BuildingBlocks.Common;
using BuildingBlocks.Events;
using ServiceDefaults;

namespace BuildingBlocks.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WebApplicationBuilder AddApiServices(this WebApplicationBuilder builder)
        {
            builder.AddServiceDefaults();
            
            builder.Services
                .AddControllers();

            builder.Services.AddCustomProblemDetails()
                .AddDomainEvents()
                .AddBehaviors()
                .AddOpenApi();

            builder.AddCaching();

            return builder;
        }
        
        public static WebApplicationBuilder AddAssemblyServices(this WebApplicationBuilder app, Assembly assembly)
        {
            app.Services
                .AddValidatorsFromAssembly(assembly)
                .AddMediatR(x=>x.RegisterServicesFromAssembly(assembly))
                .AddEndpoints(assembly);

            return app;
        }
        
        static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            return services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(options =>
                {
                    options.CustomSchemaIds(y => y.GetRequestName());
                });
        }

        static IServiceCollection AddBehaviors(this IServiceCollection services)
        {
            return services
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
        {
            return services.AddProblemDetails(setup =>
            {
                setup.Map<InvalidOperationException>(exception =>
                    new StatusCodeProblemDetails(StatusCodes.Status409Conflict)
                    {
                        Detail = exception.Message
                    });
                setup.Map<ValidationException>(exception =>
                    new StatusCodeProblemDetails(StatusCodes.Status409Conflict)
                    {
                        Detail = exception.Message
                    });
                setup.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
                setup.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);
                setup.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
            });
        }
    }
}