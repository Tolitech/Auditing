# Tolitech.CodeGenerator.Auditing
Auditing library used in projects created by the Code Generator tool.

This project helps to discover the differences found between two entities, an old one recovered and a new one to be recorded. 

Tolitech Code Generator Tool: [http://www.tolitech.com.br](https://www.tolitech.com.br/)

Examples:

```
public class PersonEntity : AuditableEntity
{
    public PersonEntity(string name, int age, string email)
    {
        using var scope = new AuditScope(this, EventTypeEnum.Insert);
        Name = name;
        Age = age;
        Email = new EmailValueObject(email, name);
    }

    public string Name { get; set; }

    public int Age { get; set; }

    public EmailValueObject Email { get; set; }

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
}
```

```
Audit.EnableEntitity(typeof(PersonEntity).FullName);
bool b = Audit.IsEntityEnabled(typeof(PersonEntity).FullName);
```

```
var person = new PersonEntity()
{
    Name = "Person One",
    Age = 18
};

person.ChangeName("Person Two");
person.ChangeAge(21);
var audit = person.Audit();

audit.AttributesDiff[0].Name == "Name"; // this is true 
audit.AttributesDiff[0].Old == "Person One"; // this is true 
audit.AttributesDiff[0].New == "Person Two"; // this is true 

audit.AttributesDiff[1].Name == "Age"; // this is true 
audit.AttributesDiff[1].Old == "18"; // this is true 
audit.AttributesDiff[1].New == "21"; // this is true 
```
