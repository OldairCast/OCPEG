using System.Runtime.Serialization;
namespace OCPEG.Framework.Exception
{
    [Serializable]
    public class OcBaseException : System.Exception
    {
        public OcBaseException()
        {
        } 

        public OcBaseException(string message)
            : base(message)
        {
        }

        public OcBaseException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        protected OcBaseException(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
