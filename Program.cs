using System;
using System.IO;
using FileHelpers;

namespace _2020Election
{
    [DelimitedRecord(",")]
    [IgnoreFirst]
    public class PAElectionRecord
    {
        public string CountyName { get; set; }
        public string ApplicantPartyDesignation { get; set; }
        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? DateOfBirth { get; set; }
        public string MailApplicationType { get; set; }
        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? ApplicationApprovedDate { get; set; }
        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? ApplicationReturnDate { get; set; }
        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? BallotMailedDate { get; set; }
        [FieldConverter(ConverterKind.Date, "MM/dd/yyyy")]
        public DateTime? BallotReturnedDate { get; set; }
        public string StateHouseDistrict { get; set; }
        public string StateSenateDistrict { get; set; }
        public string CongressionalDistrict { get; set; }
    }
    class Program
    {

        static void Main(string[] args)
        {
            bool shouldContinue = false;
            var fileName = "./2020_General_Election_Mail_Ballot_Requests_Department_of_State.csv";
            if(args != null && args.Length == 1) fileName = args[0];
            if (!File.Exists(fileName))
            {
                Console.WriteLine("Mail in ballot request file does not exist.  Would you like to download it from https://data.pa.gov/api/views/mcba-yywm/rows.csv?accessType=DOWNLOAD ?  It is approximately 444MB. Press Y for yes, N for No.");
                shouldContinue = Console.ReadKey().KeyChar.ToString().ToUpper() == "Y";
                if (!shouldContinue) return;
                var wc = new System.Net.WebClient();
                Console.WriteLine();
                Console.WriteLine("Downloading.....Press Ctrl+C to cancel.");
                wc.DownloadFile( "https://data.pa.gov/api/views/mcba-yywm/rows.csv?accessType=DOWNLOAD", fileName);
                Console.WriteLine("Done.");
            }
            // AnalyzeFile("./ASOF20201110_2020_General_Election_Mail_Ballot_Requests_Department_of_State.csv");
            AnalyzePAFile(fileName);
        }
        static void AnalyzePAFile(string fileName)
        {
            Console.WriteLine("............................");
            Console.WriteLine("Analyzing file " + fileName);
            var engine = new FileHelperAsyncEngine<PAElectionRecord>();
            var totalRows = 0;
            var totalBallotReturned = 0;
            var totalBallotReturnedDem = 0;
            var totalBallotReturnedRep = 0;
            var totalBallotReturnedOther = 0;

            var problemBallots = 0;
            var problemBallotsDem = 0;
            var problemBallotsRep = 0;
            var problemBallotsOther = 0;

            var returnedBeforeMailedTotal = 0;
            var returnedBeforeMailedDem = 0;
            var returnedBeforeMailedRep = 0;
            var returnedBeforeMailedOther = 0;

            var returnedSameDayMailedTotal = 0;
            var returnedSameDayMailedDem = 0;
            var returnedSameDayMailedRep = 0;
            var returnedSameDayMailedOther = 0;


            var ballotReturnedWithNoMailedDateTotal = 0;
            var ballotReturnedWithNoMailedDateDem = 0;
            var ballotReturnedWithNoMailedDateRep = 0;
            var ballotReturnedWithNoMailedDateOther = 0;

            var returnedAfter3rdTotal = 0;
            var returnedAfter3rdDem = 0;
            var returnedAfter3rdRep = 0;
            var returnedAfter3rdOther = 0;
            var returnedAfter3rdButNoOtherIssues = 0;

            var returnedAfter6thTotal = 0;
            var returnedAfter6thDem = 0;
            var returnedAfter6thRep = 0;
            var returnedAfter6thOther = 0;

            var nov3rd = DateTime.Parse("11/3/2020 12:00:00 AM");
            var nov6th = DateTime.Parse("11/6/2020 12:00:00 AM");
            var maxBallotReturnDate = DateTime.MinValue;
            
            var applicationReturnedAfterBallotMailedTotal = 0;
            var applicationReturnedAfterBallotMailedDem = 0;
            var applicationReturnedAfterBallotMailedRep = 0;
            var applicationReturnedAfterBallotMailedOther = 0;

            var applicationReturnedAfterApplicationApprovedTotal = 0;
            var applicationReturnedAfterApplicationApprovedDem = 0;
            var applicationReturnedAfterApplicationApprovedRep = 0;
            var applicationReturnedAfterApplicationApprovedOther = 0;

            var applicationNotReturnedButBallotReturnedTotal = 0;
            var applicationNotReturnedButBallotReturnedDem = 0;
            var applicationNotReturnedButBallotReturnedRep = 0;
            var applicationNotReturnedButBallotReturnedOther = 0;

            var ballotsReturnedByOlderThan120 = 0;
            var ballotsReturnedByYoungerThan18 = 0;
            var ballotsMissingBirthDate = 0;
            DateTime zeroTime = new DateTime(1, 1, 1);

            using (engine.BeginReadFile(fileName))
            {
                var i = 0;

                foreach (var row in engine)
                {
                    i++;
                    totalRows += 1;
                    //Skip rows where no ballot was returned.
                    if (row.BallotReturnedDate == null) continue;
                    
                    var hasPossibleIssue = false;
                  

                    if (row.BallotReturnedDate != null) totalBallotReturned++;
                    if (row.BallotReturnedDate != null && row.ApplicantPartyDesignation == "D") totalBallotReturnedDem++;
                    if (row.BallotReturnedDate != null && row.ApplicantPartyDesignation == "R") totalBallotReturnedRep++;
                    if (row.BallotReturnedDate != null && row.ApplicantPartyDesignation != "D" && row.ApplicantPartyDesignation != "R") totalBallotReturnedOther++;

                    //Older than 120.  Age calculation includes leap years, per https://stackoverflow.com/questions/4127363/date-difference-in-years-using-c-sharp
                    var age = row.DateOfBirth.HasValue?(zeroTime+nov3rd.Subtract(row.DateOfBirth.Value)).Year - 1:0; 
                    if(age >= 120 && age < 220){
                      ballotsReturnedByOlderThan120++;
                      hasPossibleIssue = true;
                    }
                    if(!row.DateOfBirth.HasValue){
                      ballotsMissingBirthDate ++;
                       hasPossibleIssue = true;
                    }
                    if(age < 18 && age > 0){
                     ballotsReturnedByYoungerThan18++;
                     hasPossibleIssue = true;
                    }

                    //No application returned, but ballot returned
                    if (row.ApplicationReturnDate == null && row.BallotReturnedDate != null) { applicationNotReturnedButBallotReturnedTotal++; hasPossibleIssue = true; }
                    if (row.ApplicationReturnDate == null && row.BallotReturnedDate != null && row.ApplicantPartyDesignation == "D") applicationNotReturnedButBallotReturnedDem++;
                    if (row.ApplicationReturnDate == null && row.BallotReturnedDate != null && row.ApplicantPartyDesignation == "R") applicationNotReturnedButBallotReturnedRep++;
                    if (row.ApplicationReturnDate == null && row.BallotReturnedDate != null && row.ApplicantPartyDesignation != "D" && row.ApplicantPartyDesignation != "R") applicationNotReturnedButBallotReturnedOther++;


                    //Application Returned after Ballot mailed
                    if (row.ApplicationReturnDate > row.BallotMailedDate && row.BallotMailedDate != null) { applicationReturnedAfterBallotMailedTotal++; hasPossibleIssue = true; }
                    if (row.ApplicationReturnDate > row.BallotMailedDate && row.BallotMailedDate != null && row.ApplicantPartyDesignation == "D") applicationReturnedAfterBallotMailedDem++;
                    if (row.ApplicationReturnDate > row.BallotMailedDate && row.BallotMailedDate != null && row.ApplicantPartyDesignation == "R") applicationReturnedAfterBallotMailedRep++;
                    if (row.ApplicationReturnDate > row.BallotMailedDate && row.BallotMailedDate != null && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") applicationReturnedAfterBallotMailedOther++;

                    //Application Returned before Application approved
                    if (row.ApplicationReturnDate < row.ApplicationApprovedDate && row.ApplicationApprovedDate != null) { applicationReturnedAfterApplicationApprovedTotal++; hasPossibleIssue = true; }
                    if (row.ApplicationReturnDate < row.ApplicationApprovedDate && row.ApplicationApprovedDate != null && row.ApplicantPartyDesignation == "D") applicationReturnedAfterApplicationApprovedDem++;
                    if (row.ApplicationReturnDate < row.ApplicationApprovedDate && row.ApplicationApprovedDate != null && row.ApplicantPartyDesignation == "R") applicationReturnedAfterApplicationApprovedRep++;
                    if (row.ApplicationReturnDate < row.ApplicationApprovedDate && row.ApplicationApprovedDate != null && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") applicationReturnedAfterApplicationApprovedOther++;

                    //Ballots Returned without any mailed date              
                    if (row.BallotReturnedDate != null && row.BallotMailedDate == null) { ballotReturnedWithNoMailedDateTotal++; hasPossibleIssue = true; }
                    if (row.BallotReturnedDate != null && row.BallotMailedDate == null && row.ApplicantPartyDesignation == "D") ballotReturnedWithNoMailedDateDem++;
                    if (row.BallotReturnedDate != null && row.BallotMailedDate == null && row.ApplicantPartyDesignation == "R") ballotReturnedWithNoMailedDateRep++;
                    if (row.BallotReturnedDate != null && row.BallotMailedDate == null && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") ballotReturnedWithNoMailedDateOther++;

                    //Ballots Returned before they were mailed
                    if (row.BallotReturnedDate < row.BallotMailedDate) { returnedBeforeMailedTotal++; hasPossibleIssue = true; }
                    if (row.BallotReturnedDate < row.BallotMailedDate && row.ApplicantPartyDesignation == "D") returnedBeforeMailedDem++;
                    if (row.BallotReturnedDate < row.BallotMailedDate && row.ApplicantPartyDesignation == "R") returnedBeforeMailedRep++;
                    if (row.BallotReturnedDate < row.BallotMailedDate && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") returnedBeforeMailedOther++;
                    if (hasPossibleIssue) problemBallots++;

                    //Ballots Returned same day they were mailed
                    if (row.BallotReturnedDate == row.BallotMailedDate) { returnedSameDayMailedTotal++; hasPossibleIssue = true; }
                    if (row.BallotReturnedDate == row.BallotMailedDate && row.ApplicantPartyDesignation == "D") returnedSameDayMailedDem++;
                    if (row.BallotReturnedDate == row.BallotMailedDate && row.ApplicantPartyDesignation == "R") returnedSameDayMailedRep++;
                    if (row.BallotReturnedDate == row.BallotMailedDate && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") returnedSameDayMailedOther++;

                    //Calculate ballots with problem above.  Do not include potential date issues below.
                    if (hasPossibleIssue) problemBallots++;
                    if (hasPossibleIssue && row.ApplicantPartyDesignation == "D") problemBallotsDem++;
                    if (hasPossibleIssue && row.ApplicantPartyDesignation == "R") problemBallotsRep++;
                    if (hasPossibleIssue && row.ApplicantPartyDesignation != "D" && row.ApplicantPartyDesignation != "R") problemBallotsOther++;

                    //Returned after Nov 3rd
                    if (row.BallotReturnedDate > nov3rd) { returnedAfter3rdTotal++; }
                    if(!hasPossibleIssue && row.BallotReturnedDate > nov3rd) returnedAfter3rdButNoOtherIssues++;
                    if (row.BallotReturnedDate > nov3rd && row.ApplicantPartyDesignation == "D") returnedAfter3rdDem++;
                    if (row.BallotReturnedDate > nov3rd && row.ApplicantPartyDesignation == "R") returnedAfter3rdRep++;
                    if (row.BallotReturnedDate > nov3rd && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") returnedAfter3rdOther++;
                    if (row.BallotReturnedDate > maxBallotReturnDate) maxBallotReturnDate = row.BallotReturnedDate.Value;

                    //Returned after Nov 6th
                    if (row.BallotReturnedDate > nov6th) { returnedAfter6thTotal++; }
                    if (row.BallotReturnedDate > nov6th && row.ApplicantPartyDesignation == "D") returnedAfter6thDem++;
                    if (row.BallotReturnedDate > nov6th && row.ApplicantPartyDesignation == "R") returnedAfter6thRep++;
                    if (row.BallotReturnedDate > nov6th && row.ApplicantPartyDesignation != "R" && row.ApplicantPartyDesignation != "D") returnedAfter6thOther++;
                    if (row.BallotReturnedDate > maxBallotReturnDate) maxBallotReturnDate = row.BallotReturnedDate.Value;
                }
            }
            Console.WriteLine($"Total rows {totalRows}");              
            Console.WriteLine($"Total ballots returned {totalBallotReturned:N0}");
            Console.WriteLine($"Total ballots returned by Democrat applicants {totalBallotReturnedDem:N0}");
            Console.WriteLine($"Total ballots returned by Republican applicants {totalBallotReturnedRep:N0}");
            Console.WriteLine($"Total ballots returned by Independant/Other applicants {totalBallotReturnedOther:N0}");
            Console.WriteLine();           
            Console.WriteLine("Applicants get a application, then return it, then the state sends them a ballot and the applicant returns it.  One would expect mail-in ballots not to be returned by applicants who never returned an application.");
            Console.WriteLine($"Mail-in ballots for all applicants returned without any application returned: {applicationNotReturnedButBallotReturnedTotal:N0}.");
            Console.WriteLine($"Mail-in ballots for Democrat applicants returned without any application returned: {applicationNotReturnedButBallotReturnedDem:N0}.");
            Console.WriteLine($"Mail-in ballots for Republican applicants returned without any application returned: {applicationNotReturnedButBallotReturnedRep:N0}.");
            Console.WriteLine($"Mail-in ballots for Independant/Other applicants returned without any application returned: {applicationNotReturnedButBallotReturnedOther:N0}.");
            Console.WriteLine();
            Console.WriteLine("Mail-in ballots get sent to applicants, then the applicant returns them.  One would expect mail-in ballots not to be returned by applicants before they were sent by the state.");
            Console.WriteLine($"Mail-in ballots for all applicants returned before the date they were sent by the state: {returnedBeforeMailedTotal:N0}.");
            Console.WriteLine($"Mail-in ballots for Democrat applicants returned before the date they were sent by the state: {returnedBeforeMailedDem:N0}.");
            Console.WriteLine($"Mail-in ballots for Republican applicants returned on or before the date they were sent by the state: {returnedBeforeMailedRep:N0}.");
            Console.WriteLine($"Mail-in ballots for Independant/Other applicants returned on or before the date they were sent by the state: {returnedBeforeMailedOther:N0}.");
            Console.WriteLine();
            Console.WriteLine("Mail-in ballots get sent to applicants, then the applicant returns them.  One would expect mail-in ballots not to be returned by applicants on the same day they were sent by the state.");
            Console.WriteLine($"Mail-in ballots for all applicants returned on the same the date they were sent by the state: {returnedSameDayMailedTotal:N0}.");
            Console.WriteLine($"Mail-in ballots for Democrat applicants returned on the same the date they were sent by the state: {returnedSameDayMailedDem:N0}.");
            Console.WriteLine($"Mail-in ballots for Republican applicants returned on or on the same the date they were sent by the state: {returnedSameDayMailedRep:N0}.");
            Console.WriteLine($"Mail-in ballots for Independant/Other applicants returned on or on the same the date they were sent by the state: {returnedSameDayMailedOther:N0}.");
            Console.WriteLine();
            Console.WriteLine("Mail-in ballots get mailed to applicants, then the applicant returns them.  One would expect there to be a record of when all mail-in ballots were sent to applicants.");
            Console.WriteLine($"Mail-in ballots for all applicants returned without any ballot mailed date: {ballotReturnedWithNoMailedDateTotal:N0}.");
            Console.WriteLine($"Mail-in ballots for Democrat applicants returned without any ballot mailed date: {ballotReturnedWithNoMailedDateDem:N0}.");
            Console.WriteLine($"Mail-in ballots for Republican applicants returned without any ballot mailed date: {ballotReturnedWithNoMailedDateRep:N0}.");
            Console.WriteLine($"Mail-in ballots for Independant/Other applicants returned without any ballot mailed date: {ballotReturnedWithNoMailedDateOther:N0}.");
            Console.WriteLine();
            Console.WriteLine("Applicants get a application, then return it, then the state sends them a ballot.  One would expect the state not to send a ballot before they get the application. ");
            Console.WriteLine($"Cases where anyapplicant's ballot was sent to them before their application was returned: {applicationReturnedAfterBallotMailedTotal:N0}.");
            Console.WriteLine($"Cases where Democrat applicant's ballot was sent to them before their application was returned: {applicationReturnedAfterBallotMailedDem:N0}.");
            Console.WriteLine($"Cases where Republican applicant's ballot was sent to them before their application was returned: {applicationReturnedAfterBallotMailedRep:N0}.");
            Console.WriteLine($"Cases where Independant/Other applicant's ballot was sent to them  before their application was returned: {applicationReturnedAfterBallotMailedOther:N0}.");
            Console.WriteLine();
            Console.WriteLine("Applicants send in an application, then the state approves it and sends them a ballot.  One would expect the state not to approve an application before they get the application.  ");
            Console.WriteLine($"Cases where applicant's application was approved before their application was returned: {applicationReturnedAfterApplicationApprovedTotal:N0}.");
            Console.WriteLine($"Cases where Democrat applicant's  application was approved before their application was returned: {applicationReturnedAfterApplicationApprovedDem:N0}.");
            Console.WriteLine($"Cases where Republican applicant's application was approved before their application was returned: {applicationReturnedAfterApplicationApprovedRep:N0}.");
            Console.WriteLine($"Cases where Independant/Other applicant's application was approved before their application was returned: {applicationReturnedAfterApplicationApprovedOther:N0}.");
            Console.WriteLine();
            Console.WriteLine("Applicants include their birthday to match to voter roles. As the oldest person in the US was 119, one would not expect there to be any ballots returned by people 120 or older.");           
            Console.WriteLine("Dates of 1/1/1800 designate voters whose date of birth is intentionally hidden due to confidentiality concerns (typically domestic violence) and are excluded here.");
            Console.WriteLine($"Mail-in ballots with an associated birthdate 120 years old or older on Nov 3rd: {ballotsReturnedByOlderThan120:N0}.");
            Console.WriteLine($"Mail-in ballots missing birthdate: {ballotsMissingBirthDate:N0}.");
            Console.WriteLine($"Mail-in ballots with an associated birthdate younger than 18 years old on Nov 3rd: {ballotsReturnedByYoungerThan18:N0}.");
            Console.WriteLine();
            Console.WriteLine("The date by which mail-in ballots must be received has been the subject of legal arguments and a case is active with SCOTUS in Pennsylvania Democratic Party v Boockvar.");         
            Console.WriteLine("Current ruling by lower courts is that late-arriving ballots must be counted unless there is reason to believe they were not postmarked by Nov. 3rd.");   
            Console.WriteLine($"Latest date Ballot received: {maxBallotReturnDate:d}");
            Console.WriteLine($"Mail-in ballots for all applicants returned after 11/3/2020: {returnedAfter3rdTotal:N0}.");
            Console.WriteLine($"Mail-in ballots for Democrat applicants returned after 11/3/2020: {returnedAfter3rdDem:N0}.");
            Console.WriteLine($"Mail-in ballots for Republican applicants returned after 11/3/2020: {returnedAfter3rdRep:N0}.");
            Console.WriteLine($"Mail-in ballots for Independant/Other applicants returned after 11/3/2020: {returnedAfter3rdOther:N0}.");
            Console.WriteLine();
            Console.WriteLine($"Mail-in ballots for all applicants returned after 11/6/2020: {returnedAfter6thTotal:N0}.");
            Console.WriteLine($"Mail-in ballots for Democrat applicants returned after 11/6/2020: {returnedAfter6thDem:N0}.");
            Console.WriteLine($"Mail-in ballots for Republican applicants returned after 11/6/2020: {returnedAfter6thRep:N0}.");
            Console.WriteLine($"Mail-in ballots for Independant/Other applicants returned after 11/6/2020: {returnedAfter6thOther:N0}.");                       
            Console.WriteLine();
            Console.WriteLine($"Total ballots with potential issues besides post-Nov 3rd return date {problemBallots:N0} (%{((double)problemBallots / (double)totalBallotReturned) * 100D:N2})");
            Console.WriteLine($"Democrat applicant ballots with potential issues besides post-Nov 3rd return date {problemBallotsDem:N0} (%{((double)problemBallotsDem / (double)totalBallotReturnedDem) * 100D:N2})");
            Console.WriteLine($"Republican applicant ballots with potential issues besides post-Nov 3rd return date {problemBallotsRep:N0} (%{((double)problemBallotsRep / (double)totalBallotReturnedRep) * 100D:N2})");
            Console.WriteLine($"Independant/Other applicant ballots with potential issues besides post-Nov 3rd return date {problemBallotsOther:N0} (%{((double)problemBallotsOther / (double)totalBallotReturnedOther) * 100D:N2})");

            Console.WriteLine($"Total ballots with post-Nov 3rd return date but no other potential issues {returnedAfter3rdButNoOtherIssues:N0} (%{((double)returnedAfter3rdButNoOtherIssues / (double)totalBallotReturned) * 100D:N2})");
        }
    }
}
