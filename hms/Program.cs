using hms;
using hms.Models;
using hms.Repos;
using hms.Services;
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
    config.CreateMap<PatientDto, Patient>();
    config.CreateMap<DoctorDtoNew, Doctor>();
    config.CreateMap<DoctorDtoPatch, Doctor>();
    config.CreateMap<DepartmentDtoNew, Department>();
    config.CreateMap<DepartmentDtoPut, Department>();
});

// register repos
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

// register services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IUNameService, UNameService>();

// db ctx pool
builder.Services.AddDbContextPool<DbCtx>(options => options.UseNpgsql(DbCtx.ConnStr));

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
//app.UseAuthorization();

// Maps controllers to the routes defined using [Route] and [HttpGet/Post/Etc]
// If not done, swagger sees the endpoints, but they are not exposed. All return 404
app.MapControllers();

// run the server, listen for requests, call middleware and controllers' routes etc
app.Run();
