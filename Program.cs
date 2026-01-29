var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "InMemory CRUD API v1");
    c.RoutePrefix = "swagger"; // default, optional
});

app.MapControllers();

builder.Services.AddControllers();

app.UseHttpsRedirection();
app.MapControllers();


app.Run();
