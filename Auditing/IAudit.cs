using System;
using System.Collections.Generic;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public interface IAudit
    {
        void Add(AuditModel model, string sql, object param, params object[] keys);

        void Add(EventTypeEnum eventType, Type entityType, string sql, object param, params object[] keys);

        IList<AuditModel> Items { get; }
    }
}
