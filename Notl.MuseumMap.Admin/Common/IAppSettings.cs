using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.Admin.Common
{
    /// <summary>
    /// Interface for storing local values on the device.
    /// </summary>
    public interface IAppSettings
    {
        /// <summary>
        /// True if the storage contains this key value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool ContainKey(string key);

        /// <summary>
        /// Returns the value specified by the key.  Null will be returned of the object is not found.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T? GetItem<T>(string key);

        /// <summary>
        /// Removes an item from local storage.
        /// </summary>
        /// <param name="key"></param>
        void RemoveItem(string key);

        /// <summary>
        /// Stores the value defined by the key in the local device storage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetItem<T>(string key, T value);
    }
}
