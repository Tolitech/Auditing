using System;
using System.Collections.Generic;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public class Audit : IAudit
    {
        public Audit()
        {
            Items = new List<AuditModel>();
        }

        public void Add(AuditModel model, string sql, object param, params object[] keys)
        {
            model.Keys = keys;
            model.Sql = sql;
            model.Parameters = param;

            Items.Add(model);
        }

        public void Add(EventTypeEnum eventType, Type entityType, string sql, object param, params object[] keys)
        {
            var model = new AuditModel
            {
                AuditId = Guid.NewGuid(),
                EventType = eventType,
                ClassName = entityType.Name,
                Namespace = entityType.Namespace,
                Keys = keys,
                Sql = sql,
                Parameters = param
            };

            Items.Add(model);
        }

        public IList<AuditModel> Items { get; }
    }
}