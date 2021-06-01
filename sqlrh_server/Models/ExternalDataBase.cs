using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class ExternalDatabase
{
    [Key]
    public string Alias { get; set; }

    public string DBMS {get; set;}
    public string ConnectionString { get; set; }
}