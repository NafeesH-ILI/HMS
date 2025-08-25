using hms.Models;
using hms.Models.DTOs;
using hms.Repos;
using hms.Repos.Interfaces;
using hms.Services;
using hms.Services.Interfaces;
using hms.Utils;
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

builder.Services.AddAutoMapper(static config => {
    config.CreateMap<Appointment, AppointmentDtoGet>();
    config.CreateMap<PatientDtoNew, Patient>();
    config.CreateMap<PatientDtoPatch, Patient>()
        .ForAllMembers(opts => opts.Condition((src, dst, srcVal) => srcVal != null));
    config.CreateMap<Patient, PatientDtoGet>();
    config.CreateMap<DoctorDtoNew, Doctor>();
    config.CreateMap<DoctorDtoPut, Doctor>();
    config.CreateMap<DoctorDtoPatch, Doctor>()
        .ForAllMembers(opts => opts.Condition((src, dst, srcVal) => srcVal != null));
    config.CreateMap<Doctor, DoctorDtoGet>();
    config.CreateMap<DepartmentDtoNew, Department>();
    config.CreateMap<DepartmentDtoPut, Department>();
    config.CreateMap<User, UserDtoGet>()
        .ForMember(dest => dest.UName, opt => opt.MapFrom(src => src.UserName));
    config.CreateMap<TypeT<User.Types>, TypeTString<User.Types>>();
    config.CreateMap<TypeTString<User.Types>, TypeT<User.Types>>();
    config.CreateMap<TypeTString<Appointment.Statuses>, TypeT<Appointment.Statuses>>();
    config.CreateMap<TypeT<Appointment.Statuses>, TypeTString<Appointment.Statuses>>();
});

// register repos
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IPassResetRepository, PassResetRepository>();

// register services
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<INameService, NameService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPassResetService, PassResetService>();

// db ctx pool
builder.Services.AddDbContextPool<DbCtx>(options => options.UseNpgsql(Consts.ConnStr));

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
    })
    .AddEntityFrameworkStores<DbCtx>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(Consts.CookieValidityMinutes);
        options.SlidingExpiration = true;
    });

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddAuthorization(options => {});

builder.Services.AddHostedService<OtpCleanupService>();
builder.Services.AddHostedService<AppointmentAutoCancellationService>();

// finally build as WebApplication from this WebApplicationBuilder
// this is where thhose appsettings.json etc get read
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    DbCtx ctx = scope.ServiceProvider.GetRequiredService<DbCtx>();
    ctx.Database.Migrate();
}

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(Consts.WebSockKeepAliveMinutes)
});

// only include swagger UI in dev builds (not in release, final build)
if (app.Environment.IsDevelopment())
{
    // this is what generates the OpenAPI json file, and serves it
    app.UseSwagger();
    // and this serves the swagger frontend
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandler();

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
