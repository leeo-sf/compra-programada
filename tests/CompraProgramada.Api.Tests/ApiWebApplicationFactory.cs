using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSubstitute;

namespace CompraProgramada.Api.Tests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public IMediator MediatorMock { get; } = Substitute.For<IMediator>();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            var mediatorDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IMediator));
            if (mediatorDescriptor != null)
                services.Remove(mediatorDescriptor);

            services.AddSingleton(_ => MediatorMock);
        });
    }
}