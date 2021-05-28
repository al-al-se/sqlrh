using Microsoft.EntityFrameworkCore;
[Index("Id", IsUnique =true)]
public class Report
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FilePath { get; set; }
}