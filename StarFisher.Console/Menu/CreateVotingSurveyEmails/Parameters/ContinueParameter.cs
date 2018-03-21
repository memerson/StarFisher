using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.CreateVotingSurveyEmails.Parameters
{
    public class ContinueParameter : YesOrNoParameterBase
    {
        protected override string GetCallToActionText()
        {
            return
                @"This command creates two emails. One email goes to the EIA Chairperson(s) requesting them to review the voting survey. " +
                @"Send that one immediately. The other email goes to the EIA team calling them to vote. Save that one for after the EIA " +
                @"Chairperson(s) approve the voting survey. Are you ready to continue (yes or no)?";
        }
    }
}