using Cona40LiveChat;
using Cona40LiveChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add credentials
/*builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("Auth"));*/
builder.Services.Configure<AuthSettings>(
    builder.Configuration.GetSection("AUTH")
);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

// Authentication (cookie)
builder.Services.AddAuthentication("LiveChatAuth")
    .AddCookie("LiveChatAuth", options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

// Authorization (roles)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin"));
});

// ChatState
builder.Services.AddSingleton<ChatState>();

// ConnectionManager
builder.Services.AddSingleton<ConnectionManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();
app.MapHub<LiveChatHub>("/liveChatHub");

app.Run();