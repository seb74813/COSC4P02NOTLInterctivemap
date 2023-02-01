using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Notl.MuseumMap.Api.Tools
{
    /// <summary>
    /// Swagger Schema filter that injects Enum information to the client.
    /// </summary>
    public class AutoRestSchemaFilter : ISchemaFilter
    {
        /// <summary>
        /// Applies the required schema information to data types.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            // Fix for Enums to include additional enum type data.
            if (context.Type.IsEnum)
            {
                var fields = context.Type.GetFields(BindingFlags.Static | BindingFlags.Public);
                schema.Properties = null;
                schema.AllOf = null;

                var array = new OpenApiArray();
                array.AddRange(Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
                schema.Extensions.Add("x-enumNames", array);
            }

            // Fix for time span information.
            else if (context.Type == typeof(TimeSpan))
            {
                schema.Type = "string";
                schema.Format = "time-span";
                schema.Description = "Use the format [d'.']hh':'mm':'ss['.'fffffff]";
                schema.Properties = null;
            }
        }
    }


}
