using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Notl.MuseumMap.Core.Entities
{
    /// <summary>
    /// The Entity class is a common base class for all classes that are stored in CosmosDB.
    /// </summary>
    [Table("Entity")]
    public class EntityBase
    {
        /// <summary>
        /// ID of the record in the database
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The partition key for the record.
        /// </summary>
        [JsonProperty(PropertyName = "partitionKey")]
        public string? PartitionKey { get; set; }

        /// <summary>
        /// The ETAG defines a unique version of the current data.  The ETAG changes when a record is updated.
        /// </summary>
        [JsonProperty(PropertyName = "_etag")]
        public string? ETag { get; set; }

        /// <summary>
        /// When the record was first created.
        /// </summary>
        [JsonProperty(PropertyName = "created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// When the record was last updated.  Null if the record hasn't been updated.
        /// </summary>
        [JsonProperty(PropertyName = "updated")]
        public DateTime? Updated { get; set; }

        /// <summary>
        /// When the record was deleted.  Null if the record is NOT deleted.
        /// </summary>
        [JsonProperty(PropertyName = "deleted")]
        public DateTime? Deleted { get; set; }

        /// <summary>
        /// Time-stamp for the record.
        /// </summary>
        [JsonProperty(PropertyName = "_ts")]
        public int Timestamp { get; set; }
    }
}
