using Common.Infrastructure;
using Microsoft.EntityFrameworkCore;
using TodoService.DataAccess;
using TodoService.Services;

namespace TodoService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TodoDbContext>(opt =>
                opt.UseInMemoryDatabase("TodoDb"));

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
            builder.Services.AddControllers();
            builder.Services.AddHostedService<UserEventListener>();
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
