using System;
using Tolitech.CodeGenerator.Auditing.Tests.Entities;
using Xunit;

namespace Tolitech.CodeGenerator.Auditing.Tests
{
    public class AuditableEntityTest
    {
        [Fact(DisplayName = "AuditableEntity - Audit - Valid")]
        public void AuditableEntity_Audit_Valid()
        {
            var person = new PersonEntity("Person One", 18);
            var audit = person.Audit();
            
            Assert.True(audit.ClassName == "PersonEntity");
            Assert.True(audit.AttributesDiff.Count > 0);
            Assert.True(audit.Sql == null);
            Assert.True(audit.Key == null);
            Assert.True(audit.Parameters == null);
            Assert.True(audit.Time >= DateTime.Today);

            audit.AddKeys("testOne");
            audit.AddKeys("testTwo");
            Assert.True(audit.Key == "testOne,testTwo");

            audit.SetParameters(new { Param1 = "TestOne", Param2 = "TestTwo" });
            Assert.True(!string.IsNullOrEmpty(audit.Parameters));
        }

        [Fact(DisplayName = "AuditableEntity - Insert - Valid")]
        public void AuditableEntity_Insert_Valid()
        {
            var person = new PersonEntity("Person One", 18);
            var audit = person.Audit();
            Assert.True(audit.ClassName == "PersonEntity");
            Assert.True(audit.AttributesDiff.Count > 0);

            Assert.True(audit.AttributesDiff[0].Name == "Name");
            Assert.True(audit.AttributesDiff[0].Old == null);
            Assert.True(audit.AttributesDiff[0].New == "Person One");

            Assert.True(audit.AttributesDiff[1].Name == "Age");
            Assert.True(audit.AttributesDiff[1].Old == null);
            Assert.True(audit.AttributesDiff[1].New == "18");
        }

        [Fact(DisplayName = "AuditableEntity - Update - Valid")]
        public void AuditableEntity_Update_Valid()
        {
            var person = new PersonEntity()
            {
                Name = "Person One",
                Age = 18
            };

            person.ChangeName("Person Two");
            person.ChangeAge(21);
            var audit = person.Audit();

            Assert.True(audit.AttributesDiff[0].Name == "Name");
            Assert.True(audit.AttributesDiff[0].Old == "Person One");
            Assert.True(audit.AttributesDiff[0].New == "Person Two");

            Assert.True(audit.AttributesDiff[1].Name == "Age");
            Assert.True(audit.AttributesDiff[1].Old == "18");
            Assert.True(audit.AttributesDiff[1].New == "21");
        }

        [Fact(DisplayName = "AuditableEntity - Delete - Valid")]
        public void AuditableEntity_Delete_Valid()
        {
            var person = new PersonEntity()
            {
                Name = "Person One",
                Age = 18
            };

            person.Delete();
            var audit = person.Audit();

            Assert.True(audit.AttributesDiff[0].Name == "Name");
            Assert.True(audit.AttributesDiff[0].Old == "Person One");
            Assert.True(audit.AttributesDiff[0].New == null);

            Assert.True(audit.AttributesDiff[1].Name == "Age");
            Assert.True(audit.AttributesDiff[1].Old == "18");
            Assert.True(audit.AttributesDiff[1].New == null);
        }

        [Fact(DisplayName = "AuditableEntity - InsertWithValueObject - Valid")]
        public void AuditableEntity_InsertWithValueObject_Valid()
        {
            var person = new PersonEntity("Person One", 18, "test@test.com");
            var audit = person.Audit();

            Assert.True(audit.AttributesDiff[2].Name == "Email.Email");
            Assert.True(audit.AttributesDiff[2].Old == null);
            Assert.True(audit.AttributesDiff[2].New == "test@test.com");

            Assert.True(audit.AttributesDiff[3].Name == "Email.Name");
            Assert.True(audit.AttributesDiff[3].Old == null);
            Assert.True(audit.AttributesDiff[3].New == "Person One");
        }
    }
}
