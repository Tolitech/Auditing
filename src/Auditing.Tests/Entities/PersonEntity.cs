using System;
using Tolitech.CodeGenerator.Auditing.Tests.ValueObjects;

namespace Tolitech.CodeGenerator.Auditing.Tests.Entities
{
    public class PersonEntity : AuditableEntity
    {
        public PersonEntity()
        {

        }

        public PersonEntity(string name, int age)
        {
            using var scope = new AuditScope(this, EventTypeEnum.Insert);
            Name = name;
            Age = age;
        }

        public PersonEntity(string name, int age, string email)
        {
            using var scope = new AuditScope(this, EventTypeEnum.Insert);
            Name = name;
            Age = age;
            Email = new EmailValueObject(email, name);
        }

        public string? Name { get; set; }

        public int Age { get; set; }

        public EmailValueObject? Email { get; set; }

        public void ChangeName(string newName)
        {
            using var scope = new AuditScope(this, EventTypeEnum.Update);
            Name = newName;
        }

        public void ChangeAge(int newAge)
        {
            using var scope = new AuditScope(this, EventTypeEnum.Update);
            Age = newAge;
        }

        public void Delete()
        {
            using var scope = new AuditScope(this, EventTypeEnum.Delete);
        }
    }
}
