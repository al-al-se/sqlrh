using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index("Login", IsUnique =true)]
public class SqlrhUser
{
    [Key]
    public string Login {get; set;}

    public string PasswordHash {get; set;}

    public bool Admin {get; set;}

    public string Name {get; set;}

    public string EmailAddress {get; set;}

    public string XMPPAddress {get; set;}

    public string PhoneNumber {get; set;}

    public SqlrhUser(string login)
    {
        Login = login;
    }

    public void Copy(SqlrhUser other)
    {
        Name = other.Name;
        Admin = other.Admin;
        EmailAddress = other.EmailAddress;
        XMPPAddress = other.XMPPAddress;
        PhoneNumber = other.PhoneNumber;
    }
}