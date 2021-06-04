using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index("Id", IsUnique =true)]
public class AccessRule
{
    [Key]
    public int Id {get; set;}
    public Report ReportTemplate {get; set;}

    public SqlrhUser AdmittedUser {get; set;}
}