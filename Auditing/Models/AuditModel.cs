using System;
using System.Collections.Generic;

namespace Tolitech.CodeGenerator.Auditing.Models
{
    public class AuditModel
    {
        public AuditModel()
        {
            Keys = new List<object>();
        }

        public Guid AuditId { get; set; }

        public EventTypeEnum EventType { get; set; }

        public string Namespace { get; set; }

        public string ClassName { get; set; }

        public IList<object> Keys { get; set; }

        public string Sql { get; set; }

        public object Parameters { get; set; }

        public IList<AttributeDiffModel> AttributesDiff { get; set; }

        public DateTime Time { get { return DateTime.Now; } }
    }
}
