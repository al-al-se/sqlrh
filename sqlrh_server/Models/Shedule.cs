using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

[Index("Id", IsUnique =true)]
public class Shedule
{
    public int Id {get; set;}

    public Report ReportTemplate {get; set;}

    public User Recipient {get; set;}

    bool SendByEmail {get; set;}

    bool SendByXmpp {get; set;}

    bool SendByTelegram {get; set;}

    bool SendByViber {get; set;}
    public DateTime Time;

    public int RepeatAtHourOfEveryDay {get; set;}

    public int RepeatAtDayOfEveryWeek {get; set;}

    public int RepeatAtDayOfEveryMonth {get; set;}

    public int RepeatAtDayOfEveryYear {get; set;}
}