using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Chat.Model {
    public static class ObjectExtensions {

        public static string Serialize(this object o) {
            if (!o.GetType().IsSerializable) {
                return null;
            }

            StringBuilder sb = new StringBuilder(1000);
            XmlSerializer serializer = new XmlSerializer(o.GetType());

            using (TextWriter tWriter = new UTF8StringWriter(sb)) {
                serializer.Serialize(tWriter, o);
            }

            return sb.ToString();
        }
    }
}
