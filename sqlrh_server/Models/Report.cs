using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
[Index("Id", IsUnique =true)]
public class Report
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
}