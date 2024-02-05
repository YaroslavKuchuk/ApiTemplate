using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System.Text;
using HttpMultipartParser;
using Services.Interfaces.ImportExportData;

namespace Services.Implementation.ImportExportData
{
    public class InputFormatterCsv : InputFormatter
    {
        private readonly CsvFormatterOptions options;

        public InputFormatterCsv(CsvFormatterOptions csvFormatterOptions)
        {
            options = csvFormatterOptions;
            SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("text/csv"));
        }

        public override Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var type = context.ModelType;
            var request = context.HttpContext.Request;
            MediaTypeHeaderValue requestContentType = null;
            MediaTypeHeaderValue.TryParse(request.ContentType, out requestContentType);
            var result = ReadStream(type, request.Body);
            return InputFormatterResult.SuccessAsync(result);
        }

        public override bool CanRead(InputFormatterContext context)
        {
            return IsType<IImportFormatModel>(context.ModelType);
        }

        protected bool IsType<T>(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType == typeof(T))
                    return true;
            }

            return false;
        }

        protected bool IsTypeOfIEnumerable(Type type)
        {
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType == typeof(IList))
                    return true;
            }

            return false;
        }

        protected virtual object ReadStream(Type type, Stream stream)
        {
            var model = (IImportFormatModel)Activator.CreateInstance(type);

            Type itemType;
            var typeIsArray = false;
            IList list;

            var prValues = model.GetType().GetProperty("Values");
            var collectionObject = prValues.GetValue(model);
            if (collectionObject.GetType().GetGenericArguments().Length > 0)
            {
                itemType = collectionObject.GetType().GetGenericArguments()[0];
            }
            else
            {
                typeIsArray = true;
                itemType = collectionObject.GetType().GetElementType();
            }

            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(itemType);

            list = (IList)Activator.CreateInstance(constructedListType);


            var parser = new MultipartFormDataParser(stream);
            string value = parser.GetParameterValue("isOverwrite");
            Stream fileStream = parser.Files[0].Data;
            var reader = new StreamReader(fileStream);

            bool skipFirstLine = options.UseSingleLineHeaderInCsv;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values;
                var delimetr = options.CsvDelimiter.ToCharArray()[0];
                DecodeLine(line, out values, delimetr);
                var itemTypeInGeneric = list.GetType().GetTypeInfo().GenericTypeArguments[0];
                var item = Activator.CreateInstance(itemTypeInGeneric);
                var infoItems = item.GetType().GetProperties();
                if (values == null || values.Length != infoItems.Length)
                    continue;
                if (skipFirstLine)
                {
                    skipFirstLine = false;
                }
                else
                {
                    for (int i = 0; i < values.Length; i++)
                    {

                        infoItems[i].SetValue(item, ChangeType(values[i], infoItems[i].PropertyType), null);
                    }

                    list.Add(item);
                }

            }

            if (typeIsArray)
            {
                Array array = Array.CreateInstance(itemType, list.Count);

                for (int t = 0; t < list.Count; t++)
                {
                    array.SetValue(list[t], t);
                }
                return array;
            }

            prValues.SetValue(model, list);
            model.IsOverride = bool.Parse(value);

            return model;
        }

        public static bool DecodeLine(string line, out string[] fields, char delimetr)
        {
            fields = null;

            if (string.IsNullOrEmpty(line))
                return false;

            int index = 0;
            var res = new List<string>();
            while (index != line.Length)
            {
                string field;
                if (ReadField(line, delimetr, ref index, out field))
                {
                    res.Add(field);
                }
                else
                {
                    return false;
                }
            }

            if (line[line.Length - 1] == delimetr)
            {
                res.Add(string.Empty);
            }

            fields = res.ToArray();
            return true;
        }

        private static bool ReadField(string line, char delimetr, ref int index, out string field)
        {
            field = null;

            if (index >= line.Length)
                return false;

            var sb = new StringBuilder();
            int state = 0;
            while (true)
            {
                char cItem = line[index];
                char? cNext = (index + 1 < line.Length - 1) ? (char?)line[index + 1] : null;
                index++;
                switch (state)
                {
                    case 0:
                        if (cItem == '"')
                        {
                            state = 4;
                        }
                        else if (cItem == delimetr)
                        {
                            field = sb.ToString();
                            return true;
                        }
                        else
                        {
                            state = 1;
                            sb.Append(cItem);
                        }
                        break;
                    case 1:
                        if (cItem == '"')
                        {
                            return false;
                        }
                        else if (cItem == delimetr)
                        {
                            field = sb.ToString();
                            return true;
                        }
                        else
                        {
                            sb.Append(cItem);
                        }
                        break;
                    case 3:
                        if (cItem == '"')
                        {
                            state = 4;
                            sb.Append(cItem);
                        }
                        else
                        {
                            return false;
                        }
                        break;
                    case 4:
                        if (cItem == '"')
                        {
                            if (cNext != null && cNext.Value == '"')
                            {
                                state = 3;
                            }
                            else
                            {
                                state = 5;
                            }
                        }
                        else
                        {
                            sb.Append(cItem);
                        }
                        break;
                    case 5:
                        if (cItem == delimetr)
                        {
                            field = sb.ToString();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }

                if (index == line.Length)
                {
                    if (state == 1 || state == 5)
                    {
                        field = sb.ToString();
                        return true;
                    }

                    return false;
                }
            }
        }


        public static object ChangeType(object value, Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null || String.IsNullOrEmpty(value.ToString()))
                {
                    return GetDefaultValue(t);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            if (t.IsEnum)
                return Enum.Parse(t, value.ToString());

            if (t == typeof(Guid))
                return Guid.Parse(value.ToString());

            return Convert.ChangeType(value, t);
        }

        public static object GetDefaultValue(Type t)
        {
            if (t.IsValueType && Nullable.GetUnderlyingType(t) == null)
                return Activator.CreateInstance(t);
            else
                return null;
        }
    }
}
