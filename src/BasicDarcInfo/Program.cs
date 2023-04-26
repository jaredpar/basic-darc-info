using BasicDarcInfo.Util;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ClientFactory>();
builder.Services.AddScoped<DarcInfo>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapGet("/", context =>
{
    context.Response.Redirect("darc-info");
    return Task.CompletedTask;
});

app.Run();
