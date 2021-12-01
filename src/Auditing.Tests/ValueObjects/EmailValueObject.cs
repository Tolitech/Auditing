using System;
using Tolitech.CodeGenerator.Domain.ValueObjects;

namespace Tolitech.CodeGenerator.Auditing.Tests.ValueObjects
{
    public class EmailValueObject : ValueObject
    {
        public EmailValueObject(string email, string name)
        {
            Email = email;
            Name = name;
        }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}