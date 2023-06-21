using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModbusManagerLib
{
    public class ModbusException : Exception
    {
        public ModbusException()
        {
        }

        public ModbusException(string message)
          : base(message)
        {
        }

        public ModbusException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected ModbusException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
