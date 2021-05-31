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
    
    protected virtual Char Delim() => ' ';//delim

    // returning value type
    protected const string TableSign = "t";

    // if table will be returned, columns formats are need
    // <aligment><field length>:<format>
    // aliment empty - right
    // aligment '-'left
    // column formats are semicolon separated
    // t{<format c1>;< format c2>} 

    protected const string ScalarSign = "s";
    // scalar needs format too
    // s{<format>}

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

    public ISQLQueryExecutor SQLExecutor {get; set;}

     public AbstractReportBuilder(IExternalDataBaseRepository r, ISQLQueryExecutor e)
     {
        DataBases = r;
        SQLExecutor = e;
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
        int cur_pos = query.IndexOf('{');

        if (cur_pos == -1) throw new FormatException();

        string valueType = query.Substring(0,cur_pos);

        int prev_pos = cur_pos;    
        cur_pos = query.IndexOf('}',prev_pos);

        if (cur_pos == -1) throw new FormatException();

        string formatString = query.Substring(prev_pos + 1, cur_pos - prev_pos - 1);

        if (query[++cur_pos] != Delim())  throw new FormatException();

        while (query[cur_pos] == Delim()) ++cur_pos;
 
        prev_pos = cur_pos;

        while (query[cur_pos] != Delim()) ++cur_pos;

        string alias = query.Substring(prev_pos, cur_pos - prev_pos);

        var task =  DataBases.GetConnectionString(alias);
        task.RunSynchronously();
        string connectionString = task.Result;

        while (query[cur_pos] == Delim()) ++cur_pos;

        string sqlQuery = query.Substring(cur_pos);

        switch (valueType) 
        {
            case TableSign:
                var dt = SQLExecutor.ExecuteReader(connectionString,sqlQuery);
                WriteTable(dt,formatString); 
                break;
            default:
                var res = SQLExecutor.ExecuteScalar(connectionString,sqlQuery);
                WriteScalar(res, formatString);
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