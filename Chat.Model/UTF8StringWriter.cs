using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Model {
    public class UTF8StringWriter : StringWriter {
        public override Encoding Encoding {
            get { return System.Text.Encoding.UTF8; }
        }

        public UTF8StringWriter(StringBuilder sb)
            : base(sb) {
            
        }
    }
}
