using System;
using System.Reflection;
using Tolitech.CodeGenerator.Auditing.Models;
using Tolitech.CodeGenerator.Domain.Entities;
using Tolitech.CodeGenerator.Domain.ValueObjects;

namespace Tolitech.CodeGenerator.Auditing
{
    public abstract class AuditableEntity : Entity
    {
        private bool _started;
        private EventTypeEnum _eventType;
        private IList<AttributeInfo> _attributesOld;
        private IList<AttributeInfo> _attributesNew;

        public AuditableEntity()
        {
            _started = false;
            _attributesOld = new List<AttributeInfo>();
            _attributesNew = new List<AttributeInfo>();
        }

        public void StartAudit(EventTypeEnum eventType)
        {
            if (!_started)
            {
                _started = true;
                _eventType = eventType;

                if (eventType == EventTypeEnum.Update || eventType == EventTypeEnum.Delete)
                    _attributesOld = ToAttributeModel(this);
            }
        }

        public void FinishAudit()
        {
            if (_eventType == EventTypeEnum.Insert || _eventType == EventTypeEnum.Update)
                _attributesNew = ToAttributeModel(this);
        }

        public AuditInfo Audit()
        {
            var type = this.GetType();

            var audit = new AuditInfo
            {
                ClassName = type.Name,
                Namespace = type.Namespace,
                FullName = type.FullName,
                EventType = _eventType,
                AttributesDiff = GetDiff()
            };

            return audit;
        }

        private static string? ToString(object? attr)
        {
            if (attr == null)
                return null;

            return attr.ToString();
        }

        private IList<AttributeInfo> ToAttributeModel<TEntity>(TEntity entity)
        {
            IList<AttributeInfo> items = new List<AttributeInfo>();

            if (entity == null)
                return items;

            var props = entity.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (!IsBaseType(prop.PropertyType, typeof(Entity)))
                {
                    if (!IsCollection(prop.PropertyType))
                    {
                        if (IsBaseType(prop.PropertyType, typeof(ValueObject)))
                        {
                            var properties = GetPropertiesValueObject(prop);

                            foreach (var _prop in properties)
                            {
                                var objectValue = prop.GetValue(entity);

                                if (objectValue != null)
                                {
                                    items.Add(new AttributeInfo
                                    {
                                        Name = $"{prop.Name}.{_prop.Name}",
                                        Value = _prop.GetValue(objectValue)
                                    });
                                }
                            }
                        }
                        else
                        {
                            items.Add(new AttributeInfo
                            {
                                Name = prop.Name,
                                Value = prop.GetValue(entity)
                            });
                        }
                    }
                }
            }

            return items;
        }

        private IList<AttributeDiffInfo> GetDiff()
        {
            var itens = new List<AttributeDiffInfo>();

            if (_attributesOld.Count == 0 && _attributesNew.Count > 0)
            {
                foreach (var attr in _attributesNew)
                {
                    itens.Add(new AttributeDiffInfo
                    {
                        Name = attr.Name,
                        Old = null,
                        New = ToString(attr.Value)
                    });
                }
            }
            else if (_attributesOld.Count > 0 && _attributesNew.Count == 0)
            {
                foreach (var attr in _attributesOld)
                {
                    itens.Add(new AttributeDiffInfo
                    {
                        Name = attr.Name,
                        Old = ToString(attr.Value),
                        New = null
                    });
                }
            }
            else if (_attributesOld.Count > 0 && _attributesNew.Count > 0)
            {
                foreach (var attr in _attributesNew)
                {
                    var attrOld = _attributesOld.FirstOrDefault(x => x.Name == attr.Name);

                    itens.Add(new AttributeDiffInfo
                    {
                        Name = attr.Name,
                        Old = ToString(attrOld?.Value),
                        New = ToString(attr.Value)
                    });
                }

                foreach (var attr in _attributesOld)
                {
                    var attrNew = itens.FirstOrDefault(x => x.Name == attr.Name);

                    if (attrNew == null)
                    {
                        itens.Add(new AttributeDiffInfo
                        {
                            Name = attr.Name,
                            Old = ToString(attr.Value),
                            New = null
                        });
                    }
                }
            }

            itens = itens.Where(x => x.Old != x.New).ToList();
            return itens;
        }

        #region Reflection

        protected IList<PropertyInfo> GetPropertiesValueObject(PropertyInfo property)
        {
            IList<PropertyInfo> properties = new List<PropertyInfo>();

            var props = property.PropertyType.GetProperties();
            foreach (var prop in props)
            {
                if (!IsBaseType(prop.PropertyType, typeof(ValueObject)))
                {
                    if (!IsCollection(prop.PropertyType))
                    {
                        properties.Add(prop);
                    }
                }
            }

            return properties;
        }

        /// <summary>
        /// Checks whether the type is a base type.
        /// </summary>
        /// <param name="typeToCheck">type to check</param>
        /// <param name="type">type</param>
        /// <returns>is (true) or (false)</returns>
        protected bool IsBaseType(Type? typeToCheck, Type type)
        {
            if (typeToCheck == null)
                return false;

            if (typeToCheck == type)
                return true;

            return IsBaseType(typeToCheck.GetTypeInfo().BaseType, type);
        }

        /// <summary>
        /// Checks whether the object type is a collection.
        /// </summary>
        /// <param name="typeToCheck">type to check</param>
        /// <returns>is collection (true) or (false)</returns>
        protected static bool IsCollection(Type typeToCheck)
        {
            var typeInfo = typeToCheck.GetTypeInfo();

            if (typeInfo.IsGenericType)
            {
                Type type = typeInfo.GetGenericTypeDefinition();

                if (type == typeof(IEnumerable<>) || type == typeof(ICollection<>) || type == typeof(IList<>))
                    return true;
            }

            return false;
        }

        #endregion
    }
}