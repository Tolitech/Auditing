using System;
using System.Collections.Concurrent;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public class Audit : IAudit
    {
        public Audit()
        {
            Items = new ConcurrentQueue<AuditModel>();
        }

        public void Add(AuditModel model, string sql, object param, params object[] keys)
        {
            foreach (var key in keys)
                model.AddKeys(key);

            model.Sql = sql;
            model.SetParameters(param);

            Items.Enqueue(model);
        }

        public void Add(EventTypeEnum eventType, Type entityType, string sql, object param, params object[] keys)
        {
            var model = new AuditModel
            {
                EventType = eventType,
                ClassName = entityType.Name,
                Namespace = entityType.Namespace,
                FullName = entityType.FullName,
                Sql = sql
            };

            foreach (var key in keys)
                model.AddKeys(key);

            model.SetParameters(param);

            Items.Enqueue(model);
        }

        public ConcurrentQueue<AuditModel> Items { get; }
    }
}