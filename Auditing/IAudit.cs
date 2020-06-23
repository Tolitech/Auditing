using System;
using System.Collections.Concurrent;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public interface IAudit
    {
        void Add(AuditModel model, string sql, object param, params object[] keys);

        void Add(EventTypeEnum eventType, Type entityType, string sql, object param, params object[] keys);

        ConcurrentQueue<AuditModel> Items { get; }
    }
}
