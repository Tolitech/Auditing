using System;
using System.Collections.Concurrent;
using System.Reflection;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public class Audit : IAudit
    {
        private static readonly IList<string> _availableEntities = new List<string>();
        private static readonly IList<string> _entitiesEnabled = new List<string>();

        public Audit()
        {
            Items = new ConcurrentQueue<AuditInfo>();
        }

        public void Add(AuditInfo model, string? sql, object? param, params object?[] keys)
        {
            if (IsEntityEnabled(model.FullName))
            {
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        if (key != null)
                            model.AddKeys(key);
                    }
                }

                model.Sql = sql;
                
                if (param != null)
                    model.SetParameters(param);

                Items.Enqueue(model);
            }
        }

        public void Add(EventTypeEnum eventType, Type entityType, string? sql, object? param, params object?[]? keys)
        {
            if (IsEntityEnabled(entityType.FullName))
            {
                var model = new AuditInfo
                {
                    EventType = eventType,
                    ClassName = entityType.Name,
                    Namespace = entityType.Namespace,
                    FullName = entityType.FullName,
                    Sql = sql
                };

                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        if (key != null)
                            model.AddKeys(key);
                    }
                }

                if (param != null)
                    model.SetParameters(param);

                Items.Enqueue(model);
            }
        }

        public ConcurrentQueue<AuditInfo> Items { get; }

        public static void AddAssembly(string assemblyString)
        {
            var assembly = AppDomain.CurrentDomain.Load(assemblyString);
            AddAssembly(assembly);
        }

        public static void AddAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                if (t.IsSubclassOf(typeof(AuditableEntity)))
                {
                    string? fullName = t.FullName;

                    if (fullName != null)
                        _availableEntities.Add(fullName);
                }
            }
        }

        public static void EnableEntitity(string? fullName)
        {
            if (!string.IsNullOrEmpty(fullName))
            {
                lock (_entitiesEnabled)
                {
                    _entitiesEnabled.Add(fullName);
                }
            }
        }

        public static void ClearEntities()
        {
            lock (_entitiesEnabled)
            {
                _entitiesEnabled.Clear();
            }
        }

        public static bool IsEntityEnabled(string? fullName)
        {
            if (fullName == null)
                return false;

            return _entitiesEnabled.Contains(fullName);
        }

        public static IList<string> GetAvailableEntities()
        {
            return _availableEntities;
        }
    }
}