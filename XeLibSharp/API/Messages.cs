using System;
using XeLib.Internal;

namespace XeLib.API
{
    public static class Messages
    {
        public static string GetMessages( bool addTrace = false )
        {
            int len;
            Functions.GetMessagesLength(out len);
            if( len < 1 )
                return "";
            if( addTrace )
            {
                var stack = new System.Diagnostics.StackTrace();
                return string.Format(
                    "{0}\n{1}",
                    Helpers.GetMessageString( len ),
                    stack.ToString() );
            }
            else
                return Helpers.GetMessageString( len );
        }

        public static void ClearMessages()
        {
            Functions.ClearMessages();
        }

        public static string GetExceptionMessage()
        {
            int len;
            Functions.GetExceptionMessageLength(out len);
            return Helpers.GetExceptionMessageString(len);
        }
    }
}