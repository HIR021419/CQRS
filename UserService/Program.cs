using Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using UserService.DataAccess;

namespace UserService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<UserDbContext>(opt =>
                opt.UseInMemoryDatabase("UserDb"));

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            builder.Services.RegisterConsulServices(builder.Configuration.GetServiceConfig());

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.MapControllers();
            app.MapGet("/healthz", () => Results.Ok("Healthy"));

            app.Run();
        }
    }
}
