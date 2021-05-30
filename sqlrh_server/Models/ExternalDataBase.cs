using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class ExternalDatabase
{
    [Key]
    public string Alias { get; set; }
    public string connectionString { get; set; }
}