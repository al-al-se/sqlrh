using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public Task BuildAsync(string templatePth, string reportPath)
    {
        return new Task(() => Build(templatePth,reportPath));
    }

    public void Build(string templatePth, string reportPath)
    {
        BuildBeginRegex();
        BuildEndRegex();

        OpenFiles(templatePth,reportPath);

        try{

            BuildSync();
        }
        catch (Exception e)
        {
            CloseFiles();
            throw e;
        }

        CloseFiles();
    }

    protected abstract void OpenFiles(string templatePth, string reportPath);

    protected abstract void CloseFiles();

    public void BuildSync()
    {     
        while (!EndOfSource())
        {
            ParseLine(ReadLine());
        }
    }

    public abstract bool EndOfSource();

    public abstract string ReadLine();

    public StringBuilder QueryText {get; protected set;} = new StringBuilder();
    public bool QueryBeginFound {get; protected set;} = false;

    public void InitQuery()
    {
        QueryText = new StringBuilder();
        QueryBeginFound = false;
    }   

    public void ParseLine(string line)
    {
        OnNewInputLine(line);
        while (!LineParsed)
        {
            if (!QueryBeginFound)
            {
                FindQueryBegin();
            } 
            if (QueryBeginFound) 
            {
                FindQueryEnd();
            }
        }
        OnLineParsed();
    }

    public abstract void OnLineParsed();
    
    string inputLine;
    int inputLinePos;

    protected void OnNewInputLine(string s)
    {
        inputLine = s;
        inputLinePos = 0;
    }

    protected bool LineParsed => inputLinePos >= inputLine.Length;

    public abstract void Write(string s);

    protected void FindQueryBegin()
    {
        var match = BeginRegex.Match(inputLine,inputLinePos);

        if (match.Success)
        {
            var s = inputLine.Substring(inputLinePos,match.Index - inputLinePos);
            Write(s);

            inputLinePos += BeginSubstringLength;

            QueryBeginFound = true;
        } else
        {
            var s = inputLine.Substring(inputLinePos);
            Write(s);
            inputLinePos = inputLine.Length;
        }
    }

    protected void FindQueryEnd()
    {
        var match = EndRegex.Match(inputLine,inputLinePos);

        if (match.Success)
        {
            var s = inputLine.Substring(inputLinePos,match.Index - inputLinePos);
            QueryText.Append(s);

            ExecuteQuery(QueryText.ToString());

            QueryBeginFound = false;
            inputLinePos += EndSubstringLength;
        }  else
        {
            var s = inputLine.Substring(inputLinePos);
            QueryText.Append(s);
        }
    }

    public virtual void ExecuteQuery(string query)
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
                Write(dt); 
                break;
            default:
                var res = SQLQueryExecutor.ExecuteScalar(connectionString,sqlQuery);
                Write(res.ToString());
                break;
        }
    }

    public  abstract void Write(DataTable dt);
}