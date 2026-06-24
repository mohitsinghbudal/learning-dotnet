using Li_copy.DataLayer.Books;
using Li_copy.DataLayer.Roles;
using Li_copy.DataLayer.UserDLL;
using Li_copy.I_InterfaceLayer;
using Li_copy.I_InterfaceLayer.BookInterface;
using Li_copy.I_InterfaceLayer.Jwt;
using Li_copy.I_InterfaceLayer.Jwt;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.I_InterfaceLayer.Login_Sign;
using Li_copy.I_InterfaceLayer.Login_Sign;
using Li_copy.I_InterfaceLayer.LoanInterface;
using Li_copy.ServiceLayer.LoginService;
using Li_copy.I_InterfaceLayer.RoleInterface;
using Li_copy.I_InterfaceLayer.UserInterface;
using Li_copy.ServiceLayer.BookSerivces;
using Li_copy.ServiceLayer.Jwt;
using Li_copy.ServiceLayer.LoginService;
using Li_copy.Services;
using Li_copy.Services.RolesServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Data;
using System.Text;
using Li_copy.DataLayer.LoanDLL;
using Li_copy.I_InterfaceLayer.FineInterface;
using Li_copy.DataLayer.FineDLL;
using Li_copy.ServiceLayer.FineServices;
using Li_copy.ServiceLayer.LoanService;
using Li_copy.I_InterfaceLayer.NotificationInterface;
using Li_copy.ServiceLayer.NotificationService;
using Li_copy.DataLayer.NotificationDLL;
using Li_copy.I_InterfaceLayer.CategoryInterface;
using Li_copy.DataLayer.CategoryDLL;
using Li_copy.ServiceLayer.CategoryService;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//controllers added here
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//the dapper configuration here
//retrieve connection string

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings'DefaultConnection' not found.");

//register IDbConnection as transient so a new conn is created per request

builder.Services.AddTransient<IDbConnection>((sp) => new SqlConnection(connectionString));
//registerd dll to use the connection
//reduce the work reducing connecting to the database
builder.Services.AddScoped<IBooksDLL, BookDLL>();
builder.Services.AddScoped<IRolesDLL, RoleDLL>();
builder.Services.AddScoped<IUserDLL, UserDLL>();
builder.Services.AddScoped<IloanDLL, LoanDLL>();
builder.Services.AddScoped<IfineDLL, FineDLL>();
builder.Services.AddScoped<INotificationDLL, NotificationDLL>();
builder.Services.AddScoped<ICategoryDLL, CategoryDLL>();

//register service
builder.Services.AddScoped<IRolesService, RoleService>();
builder.Services.AddScoped<IBookServices, BookServices>();
builder.Services.AddScoped<ILogSignReq, UserServices>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IfineService, FineService>();
builder.Services.AddScoped<IloanService, LoanService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ICategoryServices, CategorySerive>();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        )
    };
});

// Configure CORS Policy
builder.Services.AddCors(options =>
{
    // 💡 Typo fixed here: "AllowAllLoacal" -> "AllowAllLocal"
    options.AddPolicy("AllowAllLocal", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAllLocal");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //for swagger/scalar
    app.MapScalarApiReference();
}



app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
