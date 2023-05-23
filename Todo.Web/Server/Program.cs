using Todo.Web.Server;
using Todo.Web.Server.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Configure auth
builder.AddAuthentication();
builder.Services.AddAuthorizationBuilder();

// Add razor pages to render WASM todo component
builder.Services.AddRazorPages();

// Add forwarder to make sending requests to the backend easier
builder.Services.AddHttpForwarder();

var todoUrl = builder.Configuration["TodoApiUrl"]
              ?? throw new InvalidOperationException("Todo API URL is not configured");

// Configure the HttpClient for the backend API
builder.Services.AddHttpClient<AuthClient>(client => { client.BaseAddress = new Uri(todoUrl); });

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseWebAssemblyDebugging();
else
    app.UseHsts();

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapFallbackToPage("/_Host");

// Configure APIs
app.MapAuth();
app.MapTodos(todoUrl);

app.Run();