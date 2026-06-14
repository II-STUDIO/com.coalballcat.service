using System.IO;
using System.Xml.Serialization;

namespace Coalballcat.Services
{
    /// <summary>
    /// Simple string &lt;-&gt; object serializer (XML) for mobile and PC data caching.
    /// </summary>
    public static class XMLConvert
    {
        /// <summary>Serialize an object to an XML string.</summary>
        public static string Serialize<T>(this T toSerialize)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var writer = new StringWriter();
            serializer.Serialize(writer, toSerialize);
            return writer.ToString();
        }

        /// <summary>Deserialize an XML string back into an object of type <typeparamref name="T"/>.</summary>
        public static T Deserialize<T>(this string data)
        {
            var serializer = new XmlSerializer(typeof(T));
            using var reader = new StringReader(data);
            return (T)serializer.Deserialize(reader);
        }
    }
}
