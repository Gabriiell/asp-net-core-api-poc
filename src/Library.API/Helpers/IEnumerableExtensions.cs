using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;

namespace Library.API.Helpers
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<ExpandoObject> ShapeData<TSource>(this IEnumerable<TSource> source, string fields)
        {
            if (source == null)
            {
                throw new NullReferenceException();
            }

            var expandoObjects = new List<ExpandoObject>();

            var propertiesInfo = new List<PropertyInfo>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                propertiesInfo = typeof(TSource).GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
            }
            else
            {
                var splittedFields = fields.Split(",");

                foreach (var field in splittedFields)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                    }

                    propertiesInfo.Add(propertyInfo);
                }
            }

            foreach (TSource sourceObject in source)
            {
                var dataShapedObject = new ExpandoObject();

                foreach (var propertyInfo in propertiesInfo)
                {
                    var propertyValue = propertyInfo.GetValue(sourceObject);

                    ((IDictionary<string, object>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
                }

                expandoObjects.Add(dataShapedObject);
            }

            return expandoObjects;
        }
    }
}
