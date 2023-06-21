using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModbusManagerLib
{
    public class QuantityInvalidException : ModbusException
    {
        public QuantityInvalidException()
        {
        }

        public QuantityInvalidException(string message)
          : base(message)
        {
        }

        public QuantityInvalidException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected QuantityInvalidException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
