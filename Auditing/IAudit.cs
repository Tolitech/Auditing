using System;
using System.Collections.Generic;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public interface IAudit
    {
        void Add(AuditableEntity entity, string sql, object param);

        void Add(EventTypeEnum eventType, Type entityType, string sql, object param, params object[] keys);

        IList<AuditModel> Items { get; }
    }
}
