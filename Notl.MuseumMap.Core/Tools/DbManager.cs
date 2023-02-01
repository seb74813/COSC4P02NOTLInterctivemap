using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Notl.MuseumMap.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Core.Tools
{
    /// <summary>
    /// Configuration options for the DB Manager.
    /// </summary>
    public class DbManagerOptions
    {
        /// <summary>
        /// Constructor with configuration.
        /// </summary>
        /// <param name="configuration"></param>
        public DbManagerOptions(IConfiguration configuration)
        {
            var connectionString = configuration["Database:ConnectionString"];
            var databaseName = configuration["Database:DatabaseName"];

            if(string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ApplicationException("Database:ConnectionString - Missing connection string.");
            }
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ApplicationException("Database:DatabaseName - Missing database name.");
            }

            ConnectionString = connectionString;
            DatabaseName = databaseName;
        }

        /// <summary>
        /// The connection string for the CosmosDB database.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The name of the CosmosDB database.
        /// </summary>
        public string DatabaseName { get; set; }
    }

    /// <summary>
    /// Defines the update mode to apply.
    /// </summary>
    public enum UpdateMode
    {
        /// <summary>
        /// The first updates wins, other updates will fail.
        /// </summary>
        FirstUpdateWins = 0,

        /// <summary>
        /// The first update may be overwritten by another update.
        /// </summary>
        LastUpdateWins = 1
    }

    /// <summary>
    /// The DB Manager provides a simplified access to the database.
    /// </summary>
    public class DbManager
    {
        private readonly string connectionString;
        private readonly string databaseName;
        readonly ILogger<DbManager> logger;
        private CosmosClient? client;
        private Database? database;
        readonly private Dictionary<string, Container> containers = new ();

        /// <summary>
        /// The name of the database.
        /// </summary>
        public string DatabaseName { get { return databaseName; } }

        /// <summary>
        /// Constructs the DB Manager class.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public DbManager(DbManagerOptions options, ILogger<DbManager> logger)
        {
            this.connectionString = options.ConnectionString;
            this.databaseName = options.DatabaseName;
            this.logger = logger;
        }

        /// <summary>
        /// Deletes an existing container from the database
        /// </summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public async Task DeleteContainer(string containerName)
        {
            // Get the database reference
            if (database == null)
            {
                client = new CosmosClient(connectionString);
                database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            }

            // Get the container reference
            Container container;
            if (containers.ContainsKey(containerName))
            {
                container = containers[containerName];
                containers.Remove(containerName);
            }
            else
            {
                container = database.GetContainer(containerName);
            }

            // Delete the container if found
            if(container != null)
            {
                await container.DeleteContainerAsync();
            }
        }

        /// <summary>
        /// Executes a predefined stored procedure in the system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerName"></param>
        /// <param name="procedureName"></param>
        /// <param name="partitionKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public async Task<T> ExecuteAsync<T>(string containerName, string procedureName, string partitionKey, dynamic parameters)
        {
            var container = await GetContainerAsync(containerName);
            StoredProcedureExecuteResponse<T> result;
            try
            {
                // Execute the SP
                var args = new dynamic[] { parameters };
                result = await container.Scripts.ExecuteStoredProcedureAsync<T>(procedureName, new PartitionKey(partitionKey), args, null, default);
                if (result.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ApplicationException($"ExecuteAsync failed: {result.StatusCode}");
                }
                return result;
            }
            catch (CosmosException ex)
            {
                throw new ApplicationException($"ExecuteAsync failed: {ex.StatusCode} - Error executing procedure '{procedureName} in container '{containerName}' in partition {partitionKey}.", ex);
            }
        }

        /// <summary>
        /// Gets a container reference from the database.
        /// </summary>
        /// <param name="containerName">Name of the container (table).</param>
        /// <param name="partitionKeyName">Path to the Partition field in the object (defaults to '/partition')</param>
        /// <returns></returns>
        public async Task<Container> GetContainerAsync(string containerName, string partitionKeyName = "/partitionKey")
        {
            // Get the database reference
            if (database == null)
            {
                client = new CosmosClient(connectionString);
                database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            }

            // Get the container reference
            Container container;
            if (containers.ContainsKey(containerName))
            {
                container = containers[containerName];
            }
            else
            {
                try
                {
                    container = await database.CreateContainerIfNotExistsAsync(containerName, partitionKeyName);
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"GetContainerAsync failed: {ex.StatusCode} - Error getting container '{containerName}'.", ex);
                }
            }

            return container;
        }

        /// <summary>
        /// Determine the container (table) name to use by checking for a [Table] attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetContainerName<T>()
        {
            Type type = typeof(T);
            string containerName = type.Name;
            var attributes = type.GetCustomAttributes(typeof(TableAttribute), false);
            if (attributes.Length > 0)
            {
                containerName = ((TableAttribute)attributes[0]).Name;
            }

            return containerName;
        }

        /// <summary>
        /// Creates a new object in the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task CreateAsync<T>(T item) where T : EntityBase
        {
            // Validate the input
            if(item == null)
            {
                throw new ApplicationException("CreateAsync failed: item cannot be null");
            }

            // Auto-generate a new ID and partition key as required.
            if (item.Id == Guid.Empty)
            {
                item.Id = Guid.NewGuid();
            }
            if (string.IsNullOrWhiteSpace(item.PartitionKey))
            {
                item.PartitionKey = item.Id.PartitionKey();
            }

            // Ensures the entity fields are set correctly.
            item.Created = DateTime.UtcNow;
            item.Updated = null;
            item.Deleted = null;

            var options = new ItemRequestOptions
            {
                ConsistencyLevel = ConsistencyLevel.Session
            };

            // Get the container reference and create the object in the database.
            string containerName = GetContainerName<T>();
            var container = await GetContainerAsync(containerName);
            ItemResponse<T> createResponse;
            try
            {
                createResponse = await container.CreateItemAsync(item, new PartitionKey(item.PartitionKey), options);
            }
            catch (CosmosException ex)
            {
                throw new ApplicationException($"CreateAsync failed: {ex.StatusCode} - Error creating '{containerName}' with id {item.Id} in partition {item.PartitionKey}.", ex);
            }

        }

        /// <summary>
        /// Deletes an existing object from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="forceDelete">A value of false uses a logical delete (sets 'Deleted' property).  A value of true removes the record from the database.</param>
        /// <returns></returns>
        public async Task DeleteAsync<T>(T item, bool forceDelete = false) where T : EntityBase
        {
            // Validate the input
            if (item == null)
            {
                throw new ApplicationException("DeleteAsync failed: item cannot be null");
            }
            if (item.Id == Guid.Empty)
            {
                throw new ApplicationException("DeleteAsync failed: Id cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(item.PartitionKey))
            {
                throw new ApplicationException("DeleteAsync failed: PartitionKey cannot be empty");
            }

            // Get the container reference.
            string containerName = GetContainerName<T>();
            var container = await GetContainerAsync(containerName);

            // Determine if this is a logical delete or an actual delete.
            if (forceDelete)
            {
                // Delete the object from the database.
                ItemResponse<T> deleteResponse;
                try
                {
                    deleteResponse = await container.DeleteItemAsync<T>(item.Id.ToString(), new PartitionKey(item.PartitionKey));
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"DeleteAsync failed: {ex.StatusCode} - Error deleting '{containerName}' with id {item.Id} in partition {item.PartitionKey}.", ex);
                }
            }
            else
            {
                // Update the Deleted field in the database (logical delete).
                item.Deleted = DateTime.UtcNow;

                ItemResponse<T> updateResponse;
                try
                {
                    updateResponse = await container.ReplaceItemAsync<T>(item, item.Id.ToString(), new PartitionKey(item.PartitionKey));
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"DeleteAsync failed: {ex.StatusCode} - Error updates (logical delete) '{containerName}' with id {item.Id} in partition {item.PartitionKey}.", ex);
                }

            }
        }

        /// <summary>
        /// Deletes an existing object from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="partitionKey"></param>
        /// <param name="forceDelete">A value of false uses a logical delete (sets 'Deleted' property).  A value of true removes the record from the database.</param>
        /// <returns></returns>
        public async Task DeleteAsync<T>(Guid id, string? partitionKey = null, bool forceDelete = false) where T : EntityBase
        {
            // Validate the input
            if (id == Guid.Empty)
            {
                throw new ApplicationException("DeleteAsync failed: Id cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ApplicationException("DeleteAsync failed: PartitionKey cannot be empty");
            }

            // Get the container reference
            string containerName = GetContainerName<T>();
            var container = await GetContainerAsync(containerName);

            // Determine if this is a logical delete or an actual delete.
            if (forceDelete)
            {
                // Delete the object from the database.
                ItemResponse<T> deleteResponse;
                try
                {
                    deleteResponse = await container.DeleteItemAsync<T>(id.ToString(), new PartitionKey(partitionKey));
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"DeleteAsync failed: {ex.StatusCode} - Error deleting '{containerName}' with id {id} in partition {partitionKey}.", ex);
                }
            }
            else
            {
                // Get the existing record from the database
                ItemResponse<T> readResponse;
                try
                {
                    readResponse = await container.ReadItemAsync<T>(id.ToString(), new PartitionKey(partitionKey));
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"DeleteAsync failed: {ex.StatusCode} - Error reading '{containerName}' with id {id} in partition {partitionKey}.", ex);
                }

                // Check if the record was already deleted (physical or logically).
                if (readResponse.Resource == null || readResponse.Resource.Deleted != null)
                {
                    throw new ApplicationException( $"DeleteAsync failed: Error getting object '{containerName}' with id {id} in partition {partitionKey} to delete.");
                }

                // Update the Deleted field in the database (logical delete).
                readResponse.Resource.Deleted = DateTime.UtcNow;
                ItemResponse<T> updateResponse;
                try
                {
                    updateResponse = await container.ReplaceItemAsync<T>(readResponse.Resource, readResponse.Resource.Id.ToString(), new PartitionKey(readResponse.Resource.PartitionKey));
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"DeleteAsync failed: {ex.StatusCode} - Error updating (logical delete) '{containerName}' with id {id} in partition {partitionKey}.", ex);
                }

            }
        }

        /// <summary>
        /// Update an the item in the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="mode"></param>
        /// <param name="includedDeletedRecords"></param>
        /// <returns></returns>
        public async Task UpdateAsync<T>(T item, UpdateMode mode = UpdateMode.LastUpdateWins, bool includedDeletedRecords = false) where T : EntityBase
        {
            // Validate the input
            if (item == null)
            {
                throw new ApplicationException("UpdateAsync failed: item cannot be null");
            }
            if (item.Id == Guid.Empty)
            {
                throw new ApplicationException("UpdateAsync failed: Id cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(item.PartitionKey))
            {
                throw new ApplicationException("UpdateAsync failed: PartitionKey cannot be empty");
            }

            // Get the container reference
            string containerName = GetContainerName<T>();
            var container = await GetContainerAsync(containerName);

            // Check the object isn't deleted
            if (!includedDeletedRecords)
            {
                if (item.Deleted != null)
                {
                    throw new ApplicationException($"UpdateAsync failed: The entity '{containerName}' with id {item.Id} in partition {item.PartitionKey} was already deleted.");
                }
            }

            // Update the record in the database
            item.Updated = DateTime.UtcNow;

            var options = new ItemRequestOptions
            {
                ConsistencyLevel = ConsistencyLevel.Session
            };

            // Determine the update mode to apply
            switch (mode)
            {
                case UpdateMode.FirstUpdateWins:
                    options.IfMatchEtag = item.ETag;
                    break;

                case UpdateMode.LastUpdateWins:
                default:
                    break;
            }

            ItemResponse<T> updateResponse;
            try
            {
                updateResponse = await container.ReplaceItemAsync<T>(item, item.Id.ToString(), new PartitionKey(item.PartitionKey), options);
                item.ETag = updateResponse.ETag;
            }
            catch (CosmosException ex)
            {
                throw new ApplicationException($"UpdateAsync failed: {ex.StatusCode} - Error updating '{containerName}' with id {item.Id} in partition {item.PartitionKey}.", ex);
            }
        }

        /// <summary>
        /// Gets an entity from the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="partitionKey"></param>
        /// <param name="includedDeletedRecords">Allows deleted records to be included in the query.</param>
        /// <returns></returns>
        public async Task<T?> GetAsync<T>(Guid id, string partitionKey, bool includedDeletedRecords = false) where T : EntityBase
        {
            // Validate the input
            if (id == Guid.Empty)
            {
                throw new ApplicationException("GetAsync failed: Id cannot be empty");
            }
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ApplicationException("GetAsync failed: PartitionKey cannot be empty");
            }

            // Get the container reference
            string containerName = GetContainerName<T>();
            var container = await GetContainerAsync(containerName);

            // Get the record from the database
            ItemResponse<T> readResponse;
            try
            {
                readResponse = await container.ReadItemAsync<T>(id.ToString(), new PartitionKey(partitionKey));
            }
            catch (CosmosException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw new ApplicationException($"UpdateAsync failed: {ex.StatusCode} - Error reading '{containerName}' with id {id} in partition {partitionKey}.", ex);
            }

            // Check if the record was already logically deleted (if not allowed).
            if (!includedDeletedRecords && readResponse.Resource != null && readResponse.Resource.Deleted != null)
            {
                return null;
            }

            return readResponse.Resource;
        }

        /// <summary>
        /// Executes a query on the database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlQuery"></param>
        /// <param name="parameters">Query Parameters Example @id = 1234 </param>
        /// <param name="partitionKey"></param>
        /// <param name="containerName">Overrides the name of the container</param>
        /// <returns></returns>
        public async Task<List<T>> QueryAsync<T>(string sqlQuery, Dictionary<string, object>? parameters = null, string? partitionKey = null, string? containerName = null)
        {
            // Validate the input
            if (string.IsNullOrWhiteSpace(sqlQuery))
            {
                throw new ApplicationException("QueryAsync: PartitionKey cannot be empty");
            }

            // Get the container reference
            containerName ??= GetContainerName<T>();
            var container = await GetContainerAsync(containerName);

            // Execute the query (based on use of partition key)
            FeedIterator<T> queryResponse;

            // ------------------------------
            // Output the query to the logger
            // ------------------------------
            var debugQuery = sqlQuery;
            if (parameters != null)
            {
                foreach (var key in parameters.Keys)
                {
                    var value = parameters[key];
                    if (value == null)
                    {
                        value = "null";
                    }
                    else if (value is string || value is Guid || value is DateTime || value is TimeSpan)
                    {
                        value = "'" + value + "'";
                    }
                    else if (value.GetType().IsEnum)
                    {
                        throw new ApplicationException("DBManager: Attempt to pass an enumeration instead of integer value.");
                    }
                    else
                    {
                        value = value?.ToString();
                    }
                    debugQuery = debugQuery.Replace(key, value?.ToString());
                }
            }

            logger.LogTrace("Cosmos DB Query:" + debugQuery);


            try
            {
                // Construct the query
                var query = new QueryDefinition(sqlQuery);
                if (parameters != null)
                {
                    foreach (var key in parameters.Keys)
                    {
                        query.WithParameter(key, parameters[key]);
                    }
                }

                if (partitionKey == null)
                {
                    queryResponse = container.GetItemQueryIterator<T>(query);
                }
                else
                {
                    queryResponse = container.GetItemQueryIterator<T>(query, null, new QueryRequestOptions { PartitionKey = new PartitionKey(partitionKey) });
                }

            }
            catch (CosmosException ex)
            {
                throw new ApplicationException($"QueryAsync failed: {ex.StatusCode} - Error executing query {sqlQuery} in '{containerName}' in partition {partitionKey}.", ex);
            }


            // Collect the data from the results.
            var results = new List<T>();
            while (queryResponse.HasMoreResults)
            {
                try
                {
                    foreach (var item in await queryResponse.ReadNextAsync())
                    {
                        results.Add(item);
                    }
                }
                catch (CosmosException ex)
                {
                    throw new ApplicationException($"QueryAsync failed: {ex.StatusCode} - Error executing query {sqlQuery} in '{containerName}' in partition {partitionKey}.", ex);
                }
            }

            return results;
        }
    }
}
