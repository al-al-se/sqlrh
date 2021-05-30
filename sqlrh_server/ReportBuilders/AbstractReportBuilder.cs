using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Text;
public abstract class AbstractReportBuilder : IReportBuilder
{
    // sql query in report template:
    protected virtual string OpeningTag() => "<<sqlrh";
    
    protected virtual string Delim() => " ";//delim

    // returning value type
    protected const string TableSign = "t";


    protected const string ScalarIntSign = "si";

    protected const string ScalarStringSign = "svc";

    //delim

    //external database alias

    //delim

    //sql query text

    //delim

    protected virtual string ClosingTag() => "sqlrh>>";

    protected Regex BeginRegex {get; set;}

    protected Regex EndRegex {get; set;}

    protected int BeginSubstringLength {get; set;}

    protected int EndSubstringLength {get; set;}

    protected void BuildBeginRegex()
    {
        string s = $"{OpeningTag()}{Delim()}";
        BeginSubstringLength = s.Length;
        BeginRegex = new Regex(s);
    }

    protected void BuildEndRegex()
    {
        string s = $"{Delim()}{ClosingTag()}";
        EndSubstringLength = s.Length;
        EndRegex = new Regex(s); 
    }

    public IExternalDataBaseRepository DataBases {get; set;}

     public AbstractReportBuilder(IExternalDataBaseRepository r)
     {
        DataBases = r;
     }

    Task Build(string templatePth, string reportPath)
    {
        return new Task(() => BuildSync(templatePth,reportPath));
    }

        virtual public void BuildSync(string templatePth, string reportPath)
    {
        BuildBeginRegex();
        BuildEndRegex();

        using (StreamReader source = new StreamReader(templatePth))
        {
            using(StreamWriter destination = new StreamWriter(reportPath))
            {
                BuildSync(source,destination);
            }
        }
    }

    virtual protected void BuildSync(StreamReader source, StreamWriter destination)
    {
        StringBuilder query = new StringBuilder();
        while (!source.EndOfStream)
        {
            ParseLine(source.ReadLine(), ref query, destination);
        }
    }

    protected enum SearchState
    {
        BeginNotFound,
        BeginFound,
    }
    protected virtual void ParseLine(string line, ref StringBuilder query, StreamWriter destination)
    {
        SearchState state = 
            query.ToString().Length > 0 ? 
                SearchState.BeginFound : 
                SearchState.BeginNotFound;

        for (int pos = 0, queryBegin = 0; pos < line.Length;)
        {
            if (state == SearchState.BeginNotFound)
            {
                var match = BeginRegex.Match(line,pos);
                if (match.Success)
                {
                    destination.Write(line.Substring(pos,match.Index - pos));

                    queryBegin = match.Index + BeginSubstringLength;
                    pos = queryBegin;
                    state = SearchState.BeginFound;
                    continue;
                } else
                {
                    destination.Write(line.Substring(pos));
                    break;
                }
            } 
            if (state == SearchState.BeginFound) 
            {
                var match = EndRegex.Match(line,pos);
                if (match.Success)
                {
                    query.Append(line.Substring(queryBegin,match.Index - queryBegin));
                    var result = ExecuteQuery(query.ToString());
                }
            }

        }
        destination.Write(System.Environment.NewLine);
    }

    protected virtual IEnumerable<string> ExecuteQuery(string query)
    {
        Regex delimRegex = new Regex(Delim());

        var match1 = delimRegex.Match(query);

        string valueType = query.Substring(0,match1.Index);

        int pos2 = match1.Index + Delim().Length;

        var match2 = delimRegex.Match(query, pos2);

        string alias = query.Substring(pos2,match2.Index - pos2);

        var task =  DataBases.GetConnectionString(alias);
        task.RunSynchronously();
        string connectionString = task.Result;

        int pos3 = match2.Index + Delim().Length;

        string sqlQuery = query.Substring(pos3);

        switch (valueType) 
        {
            case TableSign:
                var dt = SQLQueryExecutor.ExecuteReader(connectionString,sqlQuery);
                return PrintDataTable(dt); 
            default:
                var res = SQLQueryExecutor.ExecuteScalar(connectionString,sqlQuery);
                return new List<string>() {res.ToString()};
        }
    }

    protected virtual IEnumerable<string> PrintDataTable(DataTable dt)
    {
        List<string> result = new List<string>();
        // fill list
        return result;
    }
}