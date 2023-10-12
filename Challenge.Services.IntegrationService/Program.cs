using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Challenge.Common;
using Challenge.DataAccess;
using Challenge.Mapping;
using Challenge.ServiceCollection;
using Challenge.Services.IntegrationService.Filter;
using System.Globalization;
using System.Reflection;
using System.Text.Json.Serialization;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{

    #region Builder Object

    var builder = WebApplication.CreateBuilder(args);

    #endregion

    #region General Dependency
    
    builder.Services.AddControllers();
    builder.Services.AddScoped(typeof(Configuration));
    builder.Services.AddMemoryCache();

    #endregion

    #region Swagger Config

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1",
            new OpenApiInfo
            {
                Title = "Integration Service API",
                Version = "v1",
                Description = "Integration Service",
                Contact = new OpenApiContact()
                {
                    Email = "IntegrationService@IntegrationService.com",
                    Name = "IntegrationService API"
                },
            });
                
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
                new string[]{}
            }
        });
    });

    #endregion

    #region IoC

    //Registra contexto de la bd.
    builder.Services.AddDbContext<DataBaseContext>();
    //Registra la inversion de control
    IoC.AddDependency(builder.Services);

    #endregion

    #region Auto Mapper Config
    //Registra automapper
    var mapperConfig = new MapperConfiguration(m => { m.AddProfile(new MappingProfile()); });
    IMapper mapper = mapperConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    #endregion

    #region Basic Authentication Filter 

    builder.Services.AddAuthentication("BasicAuthentication").AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

    #endregion

    //Evita la deserialización circular
    builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);

    #region App Configuration

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<DataBaseContext>();
        context.Database.EnsureCreated();
    }


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

    app.UseHttpsRedirection();

    app.MapControllers();

    var supportedCultures = new[]
    {
        new CultureInfo("es-CO")
    };

    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("es-CO"),
        
        SupportedCultures = supportedCultures,
        
        SupportedUICultures = supportedCultures
    });

    app.Run();

    #endregion

}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}