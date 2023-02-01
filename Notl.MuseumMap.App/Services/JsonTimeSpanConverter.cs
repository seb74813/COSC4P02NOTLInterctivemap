using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notl.MuseumMap.App.Services
{
    /// <summary>
    /// TimeSpans are not serialized consistently depending on what properties are present. So this 
    /// serializer will ensure the format is maintained no matter what.
    /// </summary>
    public class JsonTimeSpanConverter : JsonConverter<TimeSpan>
    {
        /// <summary>
        /// Format: Days.Hours:Minutes:Seconds:Milliseconds
        /// </summary>
        //public const string TimeSpanFormatString = @"c";

        public override void WriteJson(JsonWriter writer, TimeSpan value, JsonSerializer serializer)
        {
            var timespanFormatted = $"{value}";
            writer.WriteValue(timespanFormatted);
        }

        /// <summary>
        /// Overrides the reading of JSON for TimeSpan data types.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="hasExistingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            _ = TimeSpan.TryParse((string?)reader.Value, out TimeSpan parsedTimeSpan);
            return parsedTimeSpan;
        }
    }
}
