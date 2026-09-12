using System.Runtime.Serialization;

namespace OCPEG.Framework.Exception
{
    [Serializable]
    public class BusinessLogicCustomException : OcBaseException
    {
        public BusinessLogicCustomException()
        {
        }

        public BusinessLogicCustomException(string message)
            : base(message)
        {
        }

        public BusinessLogicCustomException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }

        protected BusinessLogicCustomException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

    }

}
