# sqlrh
SQL database report helper

# Task
An organisation records data about its work in a database. 
A  boss wants to receive a report every month. 
And the boss give report template of report as docx or odt file.
A database administrator add sql query to the template.
Then the administrator send the template in **sqlrh** and set timetable.
**sqlrh** scheduled build a report from the template and send to the boss via email or messenger.

# Architecture
## A1
* Web UI for database administrator
* Internal store for users, templates, databases, timetables.
  * file storage for templates  
  * sql database
* sheduler
* report builder
  * base builder for abstract document format
  * markdown builder
  * html builder
  * docx builder
  * odt builder
* report sender
  * email sender
  * xmpp sender 
  * telegram sender
## A2
docker container within asp.net app with resources:
* external storage for
  * report templates
  * sqlite database file
  * database connection configs and keys
* network for web api
* network for database connection
