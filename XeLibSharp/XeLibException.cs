using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace XeLib
{
    [Serializable]
    public sealed class XeLibException : Exception
    {
        public XeLibException()
        {
            _XeditLibExceptionMessage = "";
        }

        public XeLibException(string message) : base(message)
        {
            _XeditLibExceptionMessage = "";
        }

        public XeLibException(string message, string xeMessage) : base(message)
        {
            _XeditLibExceptionMessage = xeMessage;
        }

        public XeLibException(string message, Exception innerException) : base(message, innerException)
        {
            _XeditLibExceptionMessage = "";
        }

        public XeLibException(string message, string xeMessage, Exception innerException) : base(message, innerException)
        {
            _XeditLibExceptionMessage = xeMessage;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        XeLibException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            _XeditLibExceptionMessage = info.GetString("XeditLibExceptionMessage");
        }

        string _XeditLibExceptionMessage;
        public string XeditLibExceptionMessage {
            get{
                return _XeditLibExceptionMessage;
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(info.FullTypeName);
            }
            info.AddValue("XeditLibExceptionMessage", XeditLibExceptionMessage);
            base.GetObjectData(info, context);
        }

        public override string ToString()
        {
            return base.ToString() + "\r\nException returned from XEditLib:\r\n" + XeditLibExceptionMessage;
        }
    }
}