using InterviewSetup.Data;
using InterviewSetup.Data.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Net.Http;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InterviewSetup.IntegrationTests.Setup;

public class IntegrationTest
{
    protected readonly HttpClient TestClient;
    private readonly IServiceProvider _serviceProvider;

    protected IntegrationTest()
    {
        var appFactory = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                ClearDbContext(services);
                services.AddDbContext<SetupContext>(options => options.UseInMemoryDatabase(databaseName: "SetupDb"));
            });
        });
        
        TestClient = appFactory.CreateClient();

        _serviceProvider = appFactory.Services;

        SeedDb();
    }

    private void SeedDb()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<SetupContext>();
        context.Users.Add(new User { Id = Guid.NewGuid(), Name = "Chris", Surname = "Rock" });
        context.Users.Add(new User { Id = Guid.NewGuid(), Name = "Jim", Surname = "Carrey" });
        context.Users.Add(new User { Id = Guid.NewGuid(), Name = "Ricky", Surname = "Gervais" });
        context.Users.Add(new User { Id = Guid.NewGuid(), Name = "Kevin", Surname = "Hart" });
           
        context.SaveChanges();
    }
    
    private void ClearDbContext(IServiceCollection services)
    {
        services.RemoveAll(typeof(SetupContext));
        services.RemoveAll(typeof(DbContextOptions));
        services.RemoveAll(typeof(DbContextOptions<SetupContext>));
        services.Remove(
            services.SingleOrDefault(d =>
                d.ServiceType == typeof(IDbContextOptionsConfiguration<SetupContext>))
        );
        services.RemoveAll(typeof(ServiceProviderAccessor));
    }
}
