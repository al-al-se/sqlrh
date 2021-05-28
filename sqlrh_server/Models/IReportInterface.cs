using System.Collections.Generic;

public interface IReportRepository
{
    IEnumerable<Report> All { get; }

    Report Add(string name);
}