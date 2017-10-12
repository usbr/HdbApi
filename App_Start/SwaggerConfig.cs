using System.Web.Http;
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
            // http://wmpratt.com/swagger-and-asp-net-web-api-part-1/
            configuration.EnableSwagger(c =>
            {
                c.SingleApiVersion("v0", "HDB API Data Services");
                c.PrettyPrint();
                // Include if you want to setup XML comments.
                //c.IncludeXmlComments(() => new XPathDocument(GetXmlDocumentationPath()));
                c.IncludeXmlComments(string.Format(@"{0}\bin\HdbApi.XML", System.AppDomain.CurrentDomain.BaseDirectory));
                c.DescribeAllEnumsAsStrings();
                c.OperationFilter<ExamplesOperationFilter>();
            }).EnableSwaggerUi(c => { });
                        
        }
    }
}