using System;
using CsvHelper.Configuration.Attributes;

public class Excel
{
    [Name("Gender")]
    public string Gender { get; set; }

    [Name("Id")]
    public int Id { get; set; }

    [Name("Country")]
    public string Country { get; set; }

    [Name("Age")]
    public int Age { get; set; }

    [Name("Date")]
    public DateTime Date { get; set; }

    [Name("First Name")]
    public string FirstName { get; set; }

    [Name("Last Name")]
    public string LastName { get; set; }
}