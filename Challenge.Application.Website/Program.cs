using AutoMapper;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Authorization;
using NLog;
using NLog.Web;
using Challenge.Application.Website.IoC;
using Challenge.Application.Website.Mapper;
using Challenge.Common;
using System.Globalization;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

try
{
    #region Builder Object

    var builder = WebApplication.CreateBuilder(args);

    #endregion

    #region General Dependency
    
    builder.Services.AddControllersWithViews();
    builder.Services.AddScoped(typeof(Configuration));       
    builder.Services.AddRazorPages();
    builder.Services.AddHttpContextAccessor();

    #endregion

    #region MVC Register

    builder.Services.AddMvc(options =>
    {
        options.Filters.Add(new AllowAnonymousFilter());
        options.EnableEndpointRouting = true;

    }).AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

    #endregion

    #region IoC
    
    IoCWeb.AddDependency(builder.Services);

    #endregion

    #region Views and memory cache
    
    builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

    builder.Services.AddDistributedMemoryCache();

    #endregion

    #region Auto Mapper Config
    
    MapperConfiguration mapperConfig = new MapperConfiguration(m => { m.AddProfile(new MappingProfileWeb()); });
    IMapper mapper = mapperConfig.CreateMapper();
    builder.Services.AddSingleton(mapper);

    #endregion

    #region App Configuration  

    var app = builder.Build();

    
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    
        app.UseHsts();
    }

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

    app.UseStaticFiles();

    app.UseRouting();

    app.UseRouting();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}");           
    
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