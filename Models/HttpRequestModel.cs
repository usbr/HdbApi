using System.IO;

namespace HdbApi.Models
{
    /// <summary>
    /// Represents a model that contain information and data about received HttpRequest.
    /// </summary>
    public class HttpRequestModel
    {
        /// <summary>
        /// Http verb
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// API host machine
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// Base URl
        /// </summary>
        public string PathBase { get; set; }
        /// <summary>
        /// URL
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string QueryString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Scheme { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Protocol { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Uri
        {
            get
            {
                return
                    string.IsNullOrWhiteSpace(QueryString)
                        ? Scheme + "://" + Host + PathBase + Path
                        : Scheme + "://" + Host + PathBase + Path + QueryString;
            }
        }

        #region Helpers

        public static string ConvertToString(Stream stream)
        {
            try
            {
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    var serializedStream = reader.ReadToEnd();

                    return serializedStream;
                }
            }
            finally
            {
                stream?.Dispose();
            }
        }

        #endregion
    }
}