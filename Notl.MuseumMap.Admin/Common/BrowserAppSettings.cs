using Blazored.LocalStorage;


namespace Notl.MuseumMap.Admin.Common
{
    /// <summary>
    /// Provides simple storage access to the Browser local storage service.
    /// </summary>
    public class BrowserAppSettings : IAppSettings
    {
        readonly ISyncLocalStorageService storage;

        /// <summary>
        /// Constructs the storage service.
        /// </summary>
        /// <param name="storage"></param>
        public BrowserAppSettings(ISyncLocalStorageService storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// True if the storage contains a value for the key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainKey(string key)
        {
            return storage.ContainKey(key);
        }

        /// <summary>
        /// Gets an item from storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetItem<T>(string key)
        {
            return storage.GetItem<T>(key);
        }

        /// <summary>
        /// Removes an item.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveItem(string key)
        {
            storage.RemoveItem(key);
        }

        /// <summary>
        /// Sets an item.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetItem<T>(string key, T value)
        {
            storage.SetItem<T>(key, value);
        }
    }
}
