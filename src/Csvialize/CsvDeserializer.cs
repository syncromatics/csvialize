using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CsvHelper;
using Nancy.ModelBinding;

namespace Csvialize
{
    public class CsvDeserializer : IBodyDeserializer
    {
        public bool CanDeserialize(string contentType, BindingContext context)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return false;
            }

            var contentMimeType = contentType.Split(';')[0];

            return contentMimeType.Equals(ContentTypes.Csv, StringComparison.InvariantCultureIgnoreCase);
        }

        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            using (var reader = new CsvReader(new StreamReader(bodyStream)))
            {
                reader.Configuration.IsHeaderCaseSensitive = false;

                var modelType = context.GenericType ?? context.DestinationType;
                var model = reader.GetRecords(modelType).ToList();

                context.Configuration.BodyOnly = true;

                if (context.GenericType == null)
                {
                    return model.SingleOrDefault();
                }

                var methodInfo = typeof(Enumerable).GetMethod("Cast", BindingFlags.Public | BindingFlags.Static);
                var genericMethod = methodInfo.MakeGenericMethod(modelType);
                var enumerable = genericMethod.Invoke(null, new [] { model });

                var enumerableType = typeof(IEnumerable<>).MakeGenericType(modelType);

                if (context.DestinationType == enumerableType)
                {
                    return model;
                }

                var listType = typeof(List<>).MakeGenericType(modelType);

                var destinationType = context.DestinationType;

                if ((context.DestinationType.IsAbstract || context.DestinationType.IsInterface) && enumerableType.IsAssignableFrom(listType))
                {
                    if (!enumerableType.IsAssignableFrom(listType))
                    {
                        throw new Exception("The specified destination type is abstract or an interface but is not assignable from List<T>");
                    }
                    destinationType = listType;
                }

                var ctor = destinationType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any,
                    new[] { enumerableType }, new ParameterModifier[0]);

                if (ctor == null)
                {
                    throw new Exception($"The specified destination type does not have a constructor that takes IEnumerable<{modelType.Name}> as a parameter");
                }

                var otherModel = ctor.Invoke(new[] { enumerable });

                return otherModel;
            }
        }
    }
}