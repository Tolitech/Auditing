using System;
using Xunit;
using Tolitech.CodeGenerator.Auditing.Tests.Entities;

namespace Tolitech.CodeGenerator.Auditing.Tests
{
    public class AuditTest
    {
        [Fact(DisplayName = "Audit - AddOne - Valid")]
        public void Audit_AddOne_Valid()
        {
            Audit.EnableEntitity(typeof(PersonEntity).FullName);

            var person = new PersonEntity("Person One", 18);
            var audit = new Audit();
            audit.Add(person.Audit(), null, new { param1 = "param1", param2 = "param2" }, "testOne", "testTwo", "testThree");

            Assert.True(audit.Items.Count > 0);
        }

        [Fact(DisplayName = "Audit - AddTwo - Valid")]
        public void Audit_AddTwo_Valid()
        {
            Audit.EnableEntitity(typeof(PersonEntity).FullName);

            var person = new PersonEntity("Person One", 18);
            var audit = new Audit();
            audit.Add(EventTypeEnum.Insert, person.GetType(), null, new { param1 = "param1", param2 = "param2" }, "testOne", "testTwo", "testThree");

            Assert.True(audit.Items.Count > 0);
        }

        [Fact(DisplayName = "Audit - AddAssembly - Valid")]
        public void Audit_AddAssembly_Valid()
        {
            Audit.AddAssembly("Tolitech.CodeGenerator.Auditing.Tests");
            Assert.True(Audit.GetAvailableEntities().Count > 0);
        }

        [Fact(DisplayName = "Audit - IsEntityEnabled - Valid")]
        public void Audit_IsEntityEnabled_Valid()
        {
            Audit.EnableEntitity(typeof(PersonEntity).FullName);
            Assert.True(Audit.IsEntityEnabled(typeof(PersonEntity).FullName));

            Audit.ClearEntities();
            Assert.False(Audit.IsEntityEnabled(typeof(PersonEntity).FullName));
            Assert.False(Audit.IsEntityEnabled(null));
        }
    }
}
