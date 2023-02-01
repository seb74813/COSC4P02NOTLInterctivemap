using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// System contants.
    /// </summary>
    public class Constants
    { 
        /// <summary>
        /// Past days a DJ can view previous events.
        /// </summary>
        public const int PastEventDays = 30;

        /// <summary>
        /// Date format.
        /// </summary>
        public const string DateFormat = "MMM d, yyyy";

        /// <summary>
        /// Event code string length.
        /// </summary>
        public const int EventCodeLength = 6;

        /// <summary>
        /// Default event image (dark).
        /// </summary>
        public const string DefaultEventImageDark = "/images/common/party-dark.svg";

        /// <summary>
        /// Default event image (light).
        /// </summary>
        public const string DefaultEventImageLight = "/images/common/party-light.svg";

        /// <summary>
        /// Default event image (dark).
        /// </summary>
        public const string DefaultAlbumImageDark = "/images/common/album-dark.svg";

        /// <summary>
        /// Default event image (light).
        /// </summary>
        public const string DefaultAlbumImageLight = "/images/common/album-light.svg";
    }
}
