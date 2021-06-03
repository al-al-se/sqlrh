using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index("Id", IsUnique =true)]
public class SqlrhUser
{
    [Key]
    public int id {get; set;}

    public string Login {get; set;}

    public bool Admin {get; set;}

    public string Name {get; set;}

    public string EmailAddress {get; set;}

    public string XMPPAddress {get; set;}

    public string PhoneNumber {get; set;}
}