using System.Web;

namespace HdbApi.App_Code
{
    /// <summary>
    /// Class for adding custom messages to  the IIS log
    /// </summary>
    public class HdbApiLogging
    {
        public static void LogHdbApiInfoMessage(string msg)
        {
            HttpContext.Current.Response.AppendToLog("||API-INFO:" + msg.ToUpper().Replace(' ','-') + "||");
        }

        public static void LogHdbApiErrorMessage(string msg)
        {
            HttpContext.Current.Response.AppendToLog("||API-ERROR:" + msg.ToUpper().Replace(' ', '-') + "||");
        }
    }
}