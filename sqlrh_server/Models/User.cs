using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index("Id", IsUnique =true)]
public class User
{
    [Key]
    int id {get; set;}

    string Login {get; set;}

    bool Admin {get; set;}

    string Name {get; set;}

    string EmailAddress {get; set;}

    string XMPPAddress {get; set;}

    string PhoneNumber {get; set;}
}