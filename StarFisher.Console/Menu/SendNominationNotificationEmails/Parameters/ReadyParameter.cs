using StarFisher.Console.Menu.Common.Parameters;

namespace StarFisher.Console.Menu.SendNominationNotificationEmails.Parameters
{
    public class ReadyParameter : YesOrNoParameterBase
    {
        protected override string GetCallToActionText()
        {
            return @"This operation will activate Work Offline mode in Outlook and then populate your " +
                @"Outlook Outbox with nomination notification emails for all Star Awards nominees. " +
                @"You can spotcheck the emails in your Outbox. When you want Outlook to send the notifications, " +
                @"deactivate Work Offline mode on the Send/Receive tab in Outlook. You will need to manually " +
                "open and send any notification emails you spotchecked. Do you want to proceed (yes or no)?";
        }
    }
}
