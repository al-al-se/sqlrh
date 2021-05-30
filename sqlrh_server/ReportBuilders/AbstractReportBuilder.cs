using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
public abstract class AbstractReportBuilder : IReportBuilder
{
    public IExternalDataBaseRepository DataBases {get; set;}
     public AbstractReportBuilder(IExternalDataBaseRepository r)
     {
        DataBases = r;
     }

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

    protected Regex BeginRegex()
        => new Regex($"{OpeningTag()}{Delim()}");

    protected Regex EndRegex()
        => new Regex($"{Delim()}{ClosingTag()}"); 

    Task Build(string templatePth, string reportPath)
    {
        return new Task(() => BuildSync(templatePth,reportPath));
    }

    void BuildSync(string templatePth, string reportPath)
    {
            using (Stream source = File.Open(templatePth, FileMode.Open))
            {
                using(Stream destination = File.Create(reportPath))
                {
                    string line;
                }
            }
    }


    protected virtual IEnumerable<string> ExecuteQuery(string query)
    {
        Regex delimRegex = new Regex(Delim());

        var match1 = delimRegex.Match(query);

        string valueType = query.Substring(0,match1.Index);

        int pos2 = match1.Index + Delim().Length;

        var match2 = delimRegex.Match(query, pos2);

        string alias = query.Substring(pos2,pos2 + match2.Index);

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