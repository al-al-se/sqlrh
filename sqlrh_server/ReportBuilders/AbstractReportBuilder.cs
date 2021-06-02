using System.Collections;
using System.Reflection;
using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
public abstract class AbstractReportBuilder : IReportBuilder
{
    // sql query in report template:
    public virtual string OpeningTag() => "<<sqlrh";
    
    public virtual Char Delim() => ' ';//delim

    // returning value type
    public const string TableSign = "t";

    // if table will be returned, columns formats are need
    // <aligment><field length>:<format>
    // aliment empty - right
    // aligment '-'left
    // column formats are semicolon separated
    // t{<format c1>;< format c2>} 

    public const string ScalarSign = "s";
    // scalar needs format too
    // s{<format>}

    //delim

    //external database alias

    //delim

    //sql query text

    //delim

    public virtual string ClosingTag() => "sqlrh>>";

    protected Regex BeginRegex {get; set;}

    protected Regex EndRegex {get; set;}

    protected void BuildBeginRegex()
    {
        BeginRegex = new Regex($"{OpeningTag()}({Delim()}|$)");
    }

    protected void BuildEndRegex()
    {
        EndRegex = new Regex($"(^|{Delim()}){ClosingTag()}"); 
    }

    ISQLQueryExecutor sqlExecutor;

    public ILogger Logger {get; set;}
    public IReportBuilder SetLogger(ILogger l) {Logger = l; return this;}
     public AbstractReportBuilder(ISQLQueryExecutor e)
     {
        sqlExecutor = e;
     }

    public Task BuildAsync(string templatePth, string reportPath)
    {
        Logger.LogInformation($"Start async building from {templatePth} to {reportPath}");
        return Task.Run(() => Build(templatePth,reportPath));
    }

    public void Build(string templatePth, string reportPath)
    {
        Logger.LogInformation($"Start building from {templatePth} to {reportPath}");

        try{
            BuildBeginRegex();
            BuildEndRegex();

            OpenFiles(templatePth,reportPath);

            try{

                BuildSync();
            }
            catch (Exception e)
            {
                Logger.LogInformation(
                    $"Exception at  building report  from {templatePth} to {reportPath} :" + 
                    $"{Environment.NewLine} {e.Message} {e.Source} {Environment.NewLine} {e.StackTrace}");
            }

            CloseFiles();

        } catch (Exception e)
        {
            Logger.LogInformation(
                $"Exception at  building report  from {templatePth} to {reportPath} :" + 
                $"{Environment.NewLine} {e.Message} {e.Source} {Environment.NewLine} {e.StackTrace}");
        }
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
            if (QueryBeginFound && !LineParsed) 
            {
                FindQueryEnd();
            }
        }
        OnLineParsed();
    }

    protected abstract void OnLineParsed();
    
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
            int textLength = match.Index - inputLinePos;
            var s = inputLine.Substring(inputLinePos,textLength);
            Write(s);

            inputLinePos += textLength + OpeningTag().Length + 1;

            QueryBeginFound = true;
        } else
        {
            var s = inputLine.Substring(inputLinePos);
            Write(s);
            inputLinePos = inputLine.Length;
        }
    }

    public string LastFoundQuery {get; protected set;}

    protected void FindQueryEnd()
    {
        var match = EndRegex.Match(inputLine,inputLinePos);

        if (match.Success)
        {
            int queryLength = match.Index - inputLinePos;
            var s = inputLine.Substring(inputLinePos,queryLength);
            QueryText.Append(s);

            ParseQueryParametersAndExecute(QueryText.ToString());
            
            LastFoundQuery = QueryText.ToString();
            
            QueryText.Clear();

            QueryBeginFound = false;
            
            inputLinePos += queryLength + ClosingTag().Length;

            if (inputLine[match.Index] == Delim())
            {
                inputLinePos++;
            }
        }  else
        {
            var s = inputLine.Substring(inputLinePos);
            QueryText.Append(s);
            inputLinePos += s.Length;
        }
    }

    public int DataBaseRepositotyTimeoutMilisec {get; set;}

    public IReportBuilder SetDataBaseRepositotyTimeoutMilisec(int value)
    {
        DataBaseRepositotyTimeoutMilisec = value;
        return this;
    }

    public virtual void ParseQueryParametersAndExecute(string query)
    {
        Logger.LogDebug($"Parse parameters of query '{query}'");

        QueryData q = new QueryData();

        int cur_pos = 0, prev_pos = 0;

        if (!ParseValueType(query,ref prev_pos, ref cur_pos,q)) {return;}

        if (!ParseFormat(query,ref prev_pos, ref cur_pos,q)) {return;}

        if (!ParseAlias(query,ref prev_pos, ref cur_pos,q)) {return;}

        if (!ParseSQLQuery(query,ref prev_pos, ref cur_pos,q)) {return;}

        ExecuteQuery(q);
    }

    public bool ParseValueType(string query, ref int prev_pos, ref int cur_pos, QueryData q)
    {
        query.IndexOf('{');

        if (cur_pos == -1) 
        {
            string msg = "ParseValueType: { not found";
            Logger.LogError($"In query '{query}' error {msg}");
            Write(msg);
            return false;
        }

        q.valueType = query.Substring(0,cur_pos);

        return true;
    }

    public bool ParseFormat(string query, ref int prev_pos, ref int cur_pos, QueryData q)
    {
        cur_pos = query.IndexOf('}',prev_pos);

        if (cur_pos == -1)
        {
            string msg = "} not found";
            Logger.LogError($"In query '{query}' error {msg}");
            Write(msg);
            return false;
        }

        q.formatString = query.Substring(prev_pos + 1, cur_pos - prev_pos - 1);

        return true;
    }

    public bool ParseAlias(string query, ref int prev_pos, ref int cur_pos, QueryData q)
    {
        if (query[++cur_pos] != Delim())
        {
            string msg = "delimeter after format not found";
            Logger.LogError($"In query '{query}' error {msg}");
            Write(msg);
            return false;
        }

        while (query[cur_pos] == Delim()) ++cur_pos;
 
        prev_pos = cur_pos;

        while (query[cur_pos] != Delim()) ++cur_pos;

        q.alias = query.Substring(prev_pos, cur_pos - prev_pos);

        return true;
    }

    public bool ParseSQLQuery(string query, ref int prev_pos, ref int cur_pos, QueryData q)
    {
        while (query[cur_pos] == Delim()) ++cur_pos;

        q.sqlQuery = query.Substring(cur_pos);

        return true;
    }

    public void ExecuteQuery(QueryData q)
    {
        switch (q.valueType) 
        {
            case TableSign:
                var dt = sqlExecutor.ExecuteReader(q);
                WriteTable(dt,q.formatString); 
                break;
            default:
                var res = sqlExecutor.ExecuteScalar(q);
                WriteScalar(res, q.formatString);
                break;
        }
    }

    public  void WriteScalar(object o, string format)
    {
        if (o != null)
        {
            string fullFormatString = $"{{0{format}}}";
            if (format.Contains("d"))
            {
                int iv = Int32.Parse(o.ToString());
                Write(String.Format(fullFormatString,iv));
                return;
            }
            var r = new Regex("e|f");
            if (r.IsMatch(format))
            {
                double dv = Double.Parse(o.ToString());
                Write(String.Format(fullFormatString,dv));
                return;
            }
            Write(o.ToString());
            return;
        }
    }
    public  abstract void WriteTable(DataTable dt, string format);
}