using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tolitech.CodeGenerator.Auditing.Models
{
    public class AuditInfo
    {
        private readonly IList<object> _keys;
        private object? _parameters;

        public AuditInfo()
        {
            _keys = new List<object>();
            AttributesDiff = new List<AttributeDiffInfo>();
        }

        public EventTypeEnum EventType { get; set; }

        public string? Namespace { get; set; }

        public string? ClassName { get; set; }

        public string? FullName { get; set; }

        public string? Sql { get; set; }

        public IList<AttributeDiffInfo> AttributesDiff { get; set; }

        public string? Key
        {
            get
            {
                if (_keys.Count > 0)
                    return string.Join(",", _keys);

                return null;
            }
        }

        public string? Parameters
        {
            get
            {
                if (_parameters != null)
                    return JsonSerializer.Serialize(_parameters, new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, ReferenceHandler = ReferenceHandler.Preserve });

                return null;
            }
        }

        public DateTime Time { get { return DateTime.Now; } }

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