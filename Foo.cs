using CsvHelper.Configuration.Attributes;

public class Foo
{
    [Name("Username")]
    public string Username { get; set; }

    [Index(1)]
    public int Identifier { get; set; }

    [Name("First name")]
    public string FirstName { get; set; }

    [Name("Last name")]
    public string LastName { get; set; }
}