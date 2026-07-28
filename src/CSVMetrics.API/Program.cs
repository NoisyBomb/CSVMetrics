using CSVMetrics.Infrastructure;
using CSVMetrics.Application;
using CSVMetrics.API;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CsvParser>();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CsvParser>();
builder.Services.AddScoped<CsvValidator>();
builder.Services.AddScoped<AggregateCalculator>();
builder.Services.AddScoped<CsvUploadService>();
builder.Services.AddScoped<ResultsQueryService>();
builder.Services.AddScoped<RecentValuesQueryService>();


var app = builder.Build();
app.UseMiddleware<ExceptionHandler>();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();