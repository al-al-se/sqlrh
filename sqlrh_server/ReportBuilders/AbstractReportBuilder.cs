using System.Text.RegularExpressions;
public abstract class AbstractReportBuilder : IReportBuilder
{
    // sql query in report template:
    protected virtual string OpeningTag() => "<<sqlrh";
    
    protected virtual string Delim() => " ";//delim

    protected virtual string TableSign() => "t";

    //delim

    protected virtual string ScalarIntSign() => "si";

    //delim
    //sql query text

    protected virtual string ClosingTag() => "sqlrh>>";

    protected Regex SingleLineRegex() 
        => new Regex($"{OpeningTag()}{Delim()}.*{Delim()}{ClosingTag()}");

    protected Regex MultiLineBeginReges()
        => new Regex($"{OpeningTag()}{Delim()}.*$");

    protected Regex MultiLineEndReges()
        => new Regex($"^.*{Delim()}{ClosingTag()}"); 
}