using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Tolitech.CodeGenerator.Auditing.Models;

namespace Tolitech.CodeGenerator.Auditing
{
    public class Audit : IAudit
    {
        private static IList<string> _availableEntities = new List<string>();
        private static IList<string> _entitiesEnabled = new List<string>();

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
                    _availableEntities.Add(assembly.FullName);
            }
        }

        public static void EnableEntitity(string fullName)
        {
            _entitiesEnabled.Add(fullName);
        }

        public static void ClearEntities()
        {
            _entitiesEnabled.Clear();
        }

        public static bool IsEntityEnabled(string fullName)
        {
            return _availableEntities.Contains(fullName);
        }

        public static IList<string> GetAvailableEntities()
        {
            return _availableEntities;
        }
    }
}