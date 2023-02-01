using Newtonsoft.Json;
using Notl.MuseumMap.App.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Notl.MuseumMap.Mobile.Services
{
    /// <summary>
    /// Storage servcie for mobile devices.
    /// </summary>
    public class MobileAppSettings : IAppSettings
    {
        /// <summary>
        /// True if the device storage contains a value for the key provided.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainKey(string key)
        {
            return Preferences.ContainsKey(key);
        }

        /// <summary>
        /// Gets an item from storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetItem<T>(string key)
        {
            var data = Preferences.Get(key, null);
            if(data == null)
            {
                return default;
            }

            if(typeof(T).IsValueType)
            {
                return (T)Convert.ChangeType(data, typeof(T));
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
        }

        /// <summary>
        /// Removes an item from storage.
        /// </summary>
        /// <param name="key"></param>
        public void RemoveItem(string key)
        {
            Preferences.Remove(key);
        }


        /// <summary>
        /// Saves an item into the storage system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetItem<T>(string key, T value)
        {
            if (typeof(T).IsValueType)
            {
                Preferences.Set(key, value?.ToString());
            }
            else
            {
                Preferences.Set(key, JsonConvert.SerializeObject(value));
            }
            
        }
    }
}
