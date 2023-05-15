
using Microsoft.EntityFrameworkCore;
using PostlyApi.Contexts;

namespace PostlyApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add a secrets.json, which contains e.g. the db password.
            builder.Configuration.AddJsonFile("secrets.json");

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDbContext<PostlyContext>(opt => opt.UseSqlServer($"Server=ubi30.informatik.uni-siegen.de;Database=Group3;User Id=Group3;Password={builder.Configuration["dbPassword"]};TrustServerCertificate=True"));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
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