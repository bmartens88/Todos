using TodoApi;
using TodoApi.Authentication;
using TodoApi.Authorization;
using TodoApi.Todos;
using TodoApi.Users;

var builder = WebApplication.CreateBuilder(args);

// Configure auth
builder.AddAuthentication();
builder.Services.AddAuthorizationBuilder().AddCurrentUserHandler();

// Add JWT token service
builder.Services.AddTokenService();

// Configure database
var connectionString = builder.Configuration.GetConnectionString("Todos") ?? "Data Source=.db/Todos.db";
builder.Services.AddSqlite<TodoDbContext>(connectionString);

// Configure identity
builder.Services.AddIdentityCore<TodoUser>()
    .AddEntityFrameworkStores<TodoDbContext>();

// State which represents the current user
builder.Services.AddCurrentUser();

// Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => o.InferSecuritySchemes());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/", () => Results.Redirect("/swagger"));

// Configure the APIs
app.MapTodos();
app.MapUsers();

app.Run();