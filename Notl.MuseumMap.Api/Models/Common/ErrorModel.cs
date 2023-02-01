
using Notl.MuseumMap.Core.Common;

namespace Notl.MuseumMap.Api.Models.Common
{
    /// <summary>
    /// Error information.
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error code (-1 indicates a general error)
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Stack trace for the error.
        /// </summary>
        public string? StackTrace { get; set; }

        /// <summary>
        /// Constructs the Error Model.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public ErrorModel(string message, int errorCode = -1)
        {
            ErrorCode = errorCode;
            Message = message;
            StackTrace = null;
        }

        /// <summary>
        /// Constructs the Error Model.
        /// </summary>
        /// <param name="ex"></param>
        public ErrorModel(Exception ex)
        {
            Message = ex.Message;
            StackTrace = ex.StackTrace;
            if(ex is MuseumMapException appException) 
            {
                ErrorCode = (int)appException.ErrorCode;
            }
            else
            {
                ErrorCode = -1;
            }
        }
    }
}
