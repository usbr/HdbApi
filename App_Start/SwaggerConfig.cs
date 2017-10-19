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
                c.SingleApiVersion("v0", "HDB Data Services API")
                    .Description(
                        "This web page serves as the main interface, documentation, and testing service for the available HDB data services. " + 
                        "This service is being developed using a technology stack comprised of Windows/.Net, IIS, Dapper, and Swagger. " +
                        "Contact the developer for feedback, data service questions, or the development of new data services within this " + 
                        "web page." +
                        "\n\r" + 
                        "This Application Programming Interface (API) is preliminary or provisional and is subject to revision. " + 
                        "It is currently in development and as such, frequent updates, downtimes, and loss of functionality are to " + 
                        "be expected. The API has not received final approval by Reclamation. No warranty, expressed or implied, is " + 
                        "made as to the functionality of the API nor shall the fact of release constitute any such warranty. " + 
                        "The API is provided on the condition that neither Reclamation nor the U.S. Government shall be held liable "+
                        "for any damages resulting from the authorized or unauthorized use of the API."
                    )
                    .TermsOfService("Terms-Of-Service Placeholder")
                    .Contact(cc => cc
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