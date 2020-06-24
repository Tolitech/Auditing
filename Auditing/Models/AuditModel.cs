using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Tolitech.CodeGenerator.Auditing.Models
{
    public class AuditModel
    {
        private IList<object> _keys;
        private object _parameters;

        public AuditModel()
        {
            _keys = new List<object>();
            AttributesDiff = new List<AttributeDiffModel>();
        }

        public EventTypeEnum EventType { get; set; }

        public string Namespace { get; set; }

        public string ClassName { get; set; }

        public string Sql { get; set; }

        public IList<AttributeDiffModel> AttributesDiff { get; set; }

        public string Keys
        {
            get
            {
                if (_keys.Count > 0)
                    return string.Join(", ", _keys);

                return null;
            }
        }

        public string Parameters
        {
            get
            {
                if (_parameters != null)
                    return JsonSerializer.Serialize(_parameters, new JsonSerializerOptions { IgnoreNullValues = true });

                return null;
            }
        }

        public DateTime Time { get { return DateTime.Now; } }

        public string FullName { get { return Namespace + "." + ClassName; } }

        public void AddKeys(object key)
        {
            _keys.Add(key);
        }

        public void SetParameters(object parameters)
        {
            _parameters = parameters;
        }
    }
}