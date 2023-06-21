using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModbusManagerLib
{
    public class FunctionCodeNotSupportedException : ModbusException
    {
        public FunctionCodeNotSupportedException()
        {
        }

        public FunctionCodeNotSupportedException(string message)
          : base(message)
        {
        }

        public FunctionCodeNotSupportedException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected FunctionCodeNotSupportedException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
