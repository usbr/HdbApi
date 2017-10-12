using System.Web.Http;
using System.Net.Http;
using Swashbuckle.Application;
using Swashbuckle.Examples;

namespace Compusight.MoveDesk.UserManagementApi.Configuration
{
    /// <summary>
    /// Represent Swagger configuration.
    /// </summary>
    public class SwaggerConfig
    {
        /// <summary>
        /// Configures Swagger API 
        /// </summary>
        /// <param name="configuration">Instance of <see cref="HttpConfiguration"/>.</param>
        public static void Configure(HttpConfiguration configuration)
        {
            // [JR] Swagger documentation path tutorial
            // Starts service in http://localhost:31614/HdbApi/index
            // http://wmpratt.com/swagger-and-asp-net-web-api-part-1/
            configuration.EnableSwagger(c => //"HdbApi/{apiVersion}/swagger", c =>
            {
                c.Schemes(new[] { "http", "https" });
                c.SingleApiVersion("v0", "HDB API Data Services")
                    .Description("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.")
                    .TermsOfService("Terms-Of-Service Placeholder")
                    .Contact(cc => cc
                        .Name("Jon Rocha")
                        .Email("jrocha@usbr.gov"))
                    .License(lc => lc
                        .Name("MIT License")
                        .Url("https://opensource.org/licenses/MIT"));
                c.PrettyPrint();
                // Setup XML comments.
                c.IncludeXmlComments(string.Format(@"{0}\bin\HdbApi.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                c.DescribeAllEnumsAsStrings();
                // Show examples from code
                c.OperationFilter<ExamplesOperationFilter>();
            }).EnableSwaggerUi(c => //"HdbApi/{*assetPath}", c => 
            {
                // custom swagger resources
                c.CustomAsset("index", HdbApi.Startup.thisAssembly, "HdbApi.SwaggerExtensions.index.html");
                c.InjectStylesheet(HdbApi.Startup.thisAssembly, "HdbApi.SwaggerExtensions.screen.css");
                c.InjectStylesheet(HdbApi.Startup.thisAssembly, "HdbApi.SwaggerExtensions.typography.css");
                c.InjectJavaScript(HdbApi.Startup.thisAssembly, "HdbApi.SwaggerExtensions.discoveryUrlSelector.js");
            });
                        
        }
    }
}