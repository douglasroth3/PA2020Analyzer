
# PA Mail-In Ballot Analyzer
This application downloads the official mail-in ballot data from https://data.pa.gov/Government-Efficiency-Citizen-Engagement/2020-General-Election-Mail-Ballot-Requests-Departm/mcba-yywm and analyzes it.  
No claims are made as to the validity of the data, reasons for listed potential issues, impact on election results, or association with current events.

## Current Output:
```
............................
Analyzing file ./2020_General_Election_Mail_Ballot_Requests_Department_of_State.csv
Total rows 3085255
Total ballots returned 2,630,157
Total ballots returned by Democrat applicants 1,703,078
Total ballots returned by Republican applicants 622,982
Total ballots returned by Independant/Other applicants 304,097

Applicants get a application, then return it, then the state sends them a ballot and the applicant returns it.  One would expect mail-in ballots not to be returned by applicants who never returned an application.
Mail-in ballots for all applicants returned without any application returned: 40.
Mail-in ballots for Democrat applicants returned without any application returned: 23.
Mail-in ballots for Republican applicants returned without any application returned: 13.
Mail-in ballots for Independant/Other applicants returned without any application returned: 4.

Mail-in ballots get sent to applicants, then the applicant returns them.  One would expect mail-in ballots not to be returned by applicants before they were sent by the state.
Mail-in ballots for all applicants returned before the date they were sent by the state: 181.
Mail-in ballots for Democrat applicants returned before the date they were sent by the state: 119.
Mail-in ballots for Republican applicants returned on or before the date they were sent by the state: 35.
Mail-in ballots for Independant/Other applicants returned on or before the date they were sent by the state: 27.

Mail-in ballots get sent to applicants, then the applicant returns them.  One would expect mail-in ballots not to be returned by applicants on the same day they were sent by the state.
Mail-in ballots for all applicants returned on the same the date they were sent by the state: 38,592.
Mail-in ballots for Democrat applicants returned on the same the date they were sent by the state: 21,598.
Mail-in ballots for Republican applicants returned on or on the same the date they were sent by the state: 11,948.
Mail-in ballots for Independant/Other applicants returned on or on the same the date they were sent by the state: 5,046.

Mail-in ballots get mailed to applicants, then the applicant returns them.  One would expect there to be a record of when all mail-in ballots were sent to applicants.
Mail-in ballots for all applicants returned without any ballot mailed date: 10,415.
Mail-in ballots for Democrat applicants returned without any ballot mailed date: 5,903.
Mail-in ballots for Republican applicants returned without any ballot mailed date: 3,178.
Mail-in ballots for Independant/Other applicants returned without any ballot mailed date: 1,334.

Applicants get a application, then return it, then the state sends them a ballot.  One would expect the state not to send a ballot before they get the application.
Cases where any applicant's ballot was sent to them before their application was returned: 4,218.
Cases where Democrat applicant's ballot was sent to them before their application was returned: 2,788.
Cases where Republican applicant's ballot was sent to them before their application was returned: 755.
Cases where Independant/Other applicant's ballot was sent to them  before their application was returned: 675.

Applicants send in an application, then the state approves it and sends them a ballot.  One would expect the state not to approve an application before they get the application.
Cases where applicant's application was approved before their application was returned: 858.
Cases where Democrat applicant's  application was approved before their application was returned: 714.
Cases where Republican applicant's application was approved before their application was returned: 56.
Cases where Independant/Other applicant's application was approved before their application was returned: 88.

Applicants include their birthday to match to voter roles. As the oldest person in the US was 119, one would not expect there to be any ballots returned by people 120 or older.
Dates of 1/1/1800 designate voters whose date of birth is intentionally hidden due to confidentiality concerns (typically domestic violence) and are excluded here.
Mail-in ballots with an associated birthdate 120 years old or older on Nov 3rd: 18.
Mail-in ballots missing birthdate: 169.
Mail-in ballots with an associated birthdate younger than 18 years old on Nov 3rd: 1.

The date by which mail-in ballots must be received has been the subject of legal arguments and a case is active with SCOTUS in Pennsylvania Democratic Party v Boockvar.
Current ruling by lower courts is that late-arriving ballots must be counted unless there is reason to believe they were not postmarked by Nov. 3rd.
Latest date Ballot received: 12/16/2020
Mail-in ballots for all applicants returned after 11/3/2020: 69,004.
Mail-in ballots for Democrat applicants returned after 11/3/2020: 37,351.
Mail-in ballots for Republican applicants returned after 11/3/2020: 19,004.
Mail-in ballots for Independant/Other applicants returned after 11/3/2020: 12,649.

Mail-in ballots for all applicants returned after 11/6/2020: 19,660.
Mail-in ballots for Democrat applicants returned after 11/6/2020: 12,659.
Mail-in ballots for Republican applicants returned after 11/6/2020: 3,869.
Mail-in ballots for Independant/Other applicants returned after 11/6/2020: 3,132.

Total ballots with potential issues besides post-Nov 3rd return date 70,347 (%2.67)
Democrat applicant ballots with potential issues besides post-Nov 3rd return date 31,150 (%1.83)
Republican applicant ballots with potential issues besides post-Nov 3rd return date 15,974 (%2.56)
Independant/Other applicant ballots with potential issues besides post-Nov 3rd return date 7,336 (%2.41)
Total ballots with post-Nov 3rd return date but no other potential issues 67,564 (%2.57)
```

## To Run the Analysis

Install .NET 5 SDK (Not just runtime) https://dotnet.microsoft.com/download/dotnet/5.0

Install git https://git-scm.com/downloads 

From a Linux, Mac, or Windows terminal, be sure you are in a project folder and then run:

```
git clone https://github.com/douglasroth3/PA2020Analyzer.git
dotnet run
```

The first time the application is run, it will offer to download data from https://data.pa.gov/api/views/mcba-yywm/rows.csv?accessType=DOWNLOAD .  If you would rather, you may manually download the data and place it in the same folder.  Be sure to name it 2020_General_Election_Mail_Ballot_Requests_Department_of_State.csv

