using Microsoft.EntityFrameworkCore;
using WebAPI.Db;

var builder = WebApplication.CreateBuilder(args);

// CORS FIX: https://learn.microsoft.com/en-us/answers/questions/1026379/access-to-fetch-been-blocked-by-cors-policy-react
builder.Services.AddCors(options =>  
{  
    options.AddDefaultPolicy(  
        policy =>  
        {  
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()  
                .AllowAnyMethod();  
        });  
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<TheDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Initialize the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<TheDbContext>();
    
    // Ensure the database is created/migrated
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();

    // Check if data exists in the database
    // Avoid duplicated inserts
    if (!dbContext.Passwords.Any())
    {
        DatabaseInitializer.InitializeDatabase(dbContext);
        dbContext.SaveChanges();
    } else {
        Console.WriteLine("Data already exists in the database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.Run();