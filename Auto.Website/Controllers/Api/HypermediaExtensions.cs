using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Auto.Website.Controllers.Api;

public static class HypermediaExtensions
{
    public static dynamic ToDynamic(this object value)
    {
        IDictionary<string, object> expando = new ExpandoObject();
        var properties = TypeDescriptor.GetProperties(value.GetType());
        foreach (PropertyDescriptor property in properties)
        {
            if (Ignore(property)) continue;
            expando.Add(property.Name, property.GetValue(value));
        }

        return (ExpandoObject) expando;
    }

    private static bool Ignore(PropertyDescriptor property)
    {
        if (property.Name == "LazyLoader") return (true);
        return property.Attributes.OfType<Newtonsoft.Json.JsonIgnoreAttribute>().Any();
    }

    public static string ToQueryString(this IEnumerable<string> arg)
    {
        var sb = new StringBuilder();
        foreach (var e in arg)
        {
            sb.Append(e + "&");
        }

        return sb.ToString().Trim('&');
    }
}