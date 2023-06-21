using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModbusManagerLib
{
    public class CRCCheckFailedException : ModbusException
    {
        public CRCCheckFailedException()
        {
        }

        public CRCCheckFailedException(string message)
          : base(message)
        {
        }

        public CRCCheckFailedException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected CRCCheckFailedException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
