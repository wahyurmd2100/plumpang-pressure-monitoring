using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ModbusManagerLib
{
    public class StartingAddressInvalidException : ModbusException
    {
        public StartingAddressInvalidException()
        {
        }

        public StartingAddressInvalidException(string message)
          : base(message)
        {
        }

        public StartingAddressInvalidException(string message, Exception innerException)
          : base(message, innerException)
        {
        }

        protected StartingAddressInvalidException(SerializationInfo info, StreamingContext context)
          : base(info, context)
        {
        }
    }
}
