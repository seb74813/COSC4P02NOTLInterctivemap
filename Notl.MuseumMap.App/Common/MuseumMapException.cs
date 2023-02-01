using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notl.MuseumMap.App.Common
{
    /// <summary>
    /// Exception class for application specific errors.
    /// </summary>
    public class MuseumMapException : Exception
    {
        /// <summary>
        /// Constructs the exception with an error code and message.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="message"></param>
        public MuseumMapException(MuseumMapErrorCode errorCode, string? message = null)
            :base(message ?? errorCode.ToString())
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Constructs the exception with an error code, inner exception and message.
        /// </summary>
        /// <param name="errorCode"></param>
        /// <param name="innerException"></param>
        /// <param name="message"></param>
        public MuseumMapException(MuseumMapErrorCode errorCode, Exception innerException, string? message = null)
            : base(message ?? errorCode.ToString(), innerException)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// The request it error code.
        /// </summary>
        public MuseumMapErrorCode ErrorCode { get; private set; }
    }
}
