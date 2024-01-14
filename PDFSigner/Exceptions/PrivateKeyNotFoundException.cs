using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFSigner.Exceptions
{
    public class PrivateKeyNotFoundException : Exception
    {
        public PrivateKeyNotFoundException(string message): base(message) {}
    }
}
