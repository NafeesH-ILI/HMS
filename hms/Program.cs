using hms.Common;
using hms.Models;
using hms.Models.DTOs;
using hms.Repos;
using hms.Repos.Interfaces;
using hms.Services;
using hms.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Registers controllers with http server.
// If not done: UseAuthorization crashes, MapControllers crashes.
// And if those are commented out too: Then Server runs, with no exposed endpoints
builder.Services.AddControllers();

// Adds the service that reads my controllers' routes, and generates OpenAPI documentation in JSON format
// which swagger UI uses
// also, this is removed in dotnet 9.0
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(config => {
    config.CreateMap<PatientDtoNew, Patient>();
    config.CreateMap<PatientDtoPatch, Patient>()
        .ForAllMembers(opts => opts.Condition((src, dst, srcVal) => srcVal != null));
    config.CreateMap<DoctorDtoNew, Doctor>();
    config.CreateMap<DoctorDtoPatch, Doctor>()
        .ForAllMembers(opts => opts.Condition((src, dst, srcVal) => srcVal != null));
    config.CreateMap<DepartmentDtoNew, Department>();
    config.CreateMap<DepartmentDtoPut, Department>();
});

// register repos
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// register services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IUNameService, UNameService>();
builder.Services.AddScoped<IUserService, UserService>();

// db ctx pool
builder.Services.AddDbContextPool<DbCtx>(options => options.UseNpgsql(DbCtx.ConnStr));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

builder.Services
    .AddAuthorization(options =>
    {
        /*foreach (User.Types role in Enum.GetValues<User.Types>())
        {
                options.AddPolicy(role.ToString(), policy => policy.RequireRole(role.ToString()));
        }*/
    });

// finally build as WebApplication from this WebApplicationBuilder
// this is where thhose appsettings.json etc get read
var app = builder.Build();

// only include swagger UI in dev builds (not in release, final build)
if (app.Environment.IsDevelopment())
{
    // this is what generates the OpenAPI json file, and serves it
    app.UseSwagger();
    // and this serves the swagger frontend
    app.UseSwaggerUI();
}

// redirects requests coming on http:// to https:// using the PermantlyMoved response code
app.UseHttpsRedirection();

// authorization middleware. we not using it right now, so does not matter
app.UseAuthentication();
app.UseAuthorization();

// Maps controllers to the routes defined using [Route] and [HttpGet/Post/Etc]
// If not done, swagger sees the endpoints, but they are not exposed. All return 404
app.MapControllers();

// run the server, listen for requests, call middleware and controllers' routes etc
app.Run();
