using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace See4Me.Engine
{
    public class CognitiveException : Exception
    {
        public CognitiveException(string message, Exception innerException = null)
            : base(message, innerException)
        { }

        public string Code { get; internal set; }

        public HttpStatusCode HttpStatusCode { get; internal set; } = HttpStatusCode.InternalServerError;

        internal CognitiveException() { }
    }
}
