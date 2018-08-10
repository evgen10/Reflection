using MyIoc.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyIoc
{
    
    public class ContainerException : Exception
    {
        public ContainerException()
        {
        }

        public ContainerException(string message) : base(message)
        {
        }

        public ContainerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
