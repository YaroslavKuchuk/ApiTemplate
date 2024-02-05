using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace Services.Implementation.ImportExportData
{
    public class OutputFormatterCsv : OutputFormatter
    {
        private readonly CsvFormatterOptions options;

        public string ContentType { get; private set; }

        public OutputFormatterCsv(CsvFormatterOptions csvFormatterOptions)
        {
            ContentType = "text/csv";
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("text/csv"));
            options = csvFormatterOptions;
            //SupportedEncodings.Add(Encoding.GetEncoding("utf-8"));
        }

        protected override bool CanWriteType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            return IsTypeOfIEnumerable(type);
        }

        private bool IsTypeOfIEnumerable(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType == typeof(IList))
                    return true;
            }
            return false;
        }

        public async override Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            var response = context.HttpContext.Response;
            Type type = context.Object.GetType();
            Type itemType;

            if (type.GetGenericArguments().Length > 0)
            {
                itemType = type.GetGenericArguments()[0];
            }
            else
            {
                itemType = type.GetElementType();
            }

            StringWriter strWriter = new StringWriter();
            if (options.UseSingleLineHeaderInCsv)
            {
                strWriter.WriteLine(
                    string.Join<string>(
                        options.CsvDelimiter, itemType.GetProperties().Select(x => 
                        x.GetCustomAttributes(typeof(JsonPropertyAttribute), true).Cast<JsonPropertyAttribute>().FirstOrDefault().PropertyName)
                    )
                );
            }

            foreach (var obj in (IEnumerable<object>)context.Object)
            {
                var items = obj.GetType().GetProperties().Select(
                    pi => new
                    {
                        Value = pi.GetValue(obj, null)
                    }
                );
                string row = items.Select(item => {
                    string value = string.Empty;
                    if (item.Value != null)
                    {
                        value = item.Value.ToString();
                        if (value.Contains(","))
                        {
                            value = string.Concat("\"", value, "\"");
                        }

                        if (value.Contains("\r"))
                        {
                            value = value.Replace("\r", " ");
                        }
                        if (value.Contains("\n"))
                        {
                            value = value.Replace("\n", " ");
                        }
                        return value;
                    }
                    return value;
                }).Aggregate((a, b) => a + "," + b);

                strWriter.WriteLine(row);
            }

            var streamWriter = new StreamWriter(response.Body);
            await streamWriter.WriteAsync(strWriter.ToString());
            await streamWriter.FlushAsync();
        }
    }
}
