using System.Runtime.Serialization;

namespace OCPEG.Framework.Exception
{
    [Serializable]
    public class OcException : OcBaseException
    {
        public OcException()
        {
        }

        public OcException(string message)
            : base(message)
        {
        }

        public OcException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        protected OcException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
