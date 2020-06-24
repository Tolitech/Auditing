using System;
using System.Collections.Generic;
using System.Linq;
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
        private IList<AttributeModel> _attributesOld;
        private IList<AttributeModel> _attributesNew;

        protected AuditableEntity()
        {
            _started = false;
            _attributesOld = new List<AttributeModel>();
            _attributesNew = new List<AttributeModel>();
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

        public AuditModel Audit()
        {
            var type = this.GetType();

            var audit = new AuditModel
            {
                ClassName = type.Name,
                Namespace = type.Namespace,
                FullName = type.FullName,
                EventType = _eventType,
                AttributesDiff = GetDiff()
            };

            return audit;
        }

        private string ToString(object attr)
        {
            if (attr == null)
                return null;

            return attr.ToString();
        }

        private IList<AttributeModel> ToAttributeModel<TEntity>(TEntity entity)
        {
            IList<AttributeModel> items = new List<AttributeModel>();

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
                                items.Add(new AttributeModel
                                {
                                    Name = _prop.Name,
                                    Value = _prop.GetValue(entity)
                                });
                            }
                        }
                        else
                        {
                            items.Add(new AttributeModel
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

        private IList<AttributeDiffModel> GetDiff()
        {
            var itens = new List<AttributeDiffModel>();

            if (_attributesOld.Count == 0 && _attributesNew.Count > 0)
            {
                foreach (var attr in _attributesNew)
                {
                    itens.Add(new AttributeDiffModel
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
                    itens.Add(new AttributeDiffModel
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

                    itens.Add(new AttributeDiffModel
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
                        itens.Add(new AttributeDiffModel
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
        protected bool IsBaseType(Type typeToCheck, Type type)
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
        protected bool IsCollection(Type typeToCheck)
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
