using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
public abstract class AbstractReportBuilder : IReportBuilder
{
    // sql query in report template:
    protected virtual string OpeningTag() => "<<sqlrh";
    
    protected virtual string Delim() => " ";//delim

    // returning value type
    protected virtual string TableSign() => "t";


    protected virtual string ScalarIntSign() => "i";

    //delim

    //external database alias

    //delim

    //sql query text

    //delim

    protected virtual string ClosingTag() => "sqlrh>>";

    protected Regex BeginRegex()
        => new Regex($"{OpeningTag()}{Delim()}");

    protected Regex EndRegex()
        => new Regex($"{Delim()}{ClosingTag()}"); 

    protected virtual IEnumerable<string> PrintDataTable(DataTable dt)
    {
        List<string> result = new List<string>();
        // fill list
        return result;
    }

    protected virtual IEnumerable<string> ExecuteQuery(string query)
    {
        Regex delimRegex = new Regex(Delim());

        var match1 = delimRegex.Match(query);

        string valueType = query.Substring(0,match1.Index);

        int pos2 = match1.Index + Delim().Length;

        var match2 = delimRegex.Match(query, pos2);

        string alias = query.Substring(pos2,pos2 + match2.Index);

        int pos3 = match2.Index + Delim().Length;

        string sqlQuery = query.Substring(pos3);

        if (valueType == TableSign()) 
        {
            return PrintDataTable(SQLQueryExecutor.ExecuteReader()); 
        }
    }
}