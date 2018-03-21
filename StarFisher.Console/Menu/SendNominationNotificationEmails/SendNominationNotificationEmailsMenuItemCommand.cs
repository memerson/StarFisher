using System;
using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.SendNominationNotificationEmails.Parameters;
using StarFisher.Domain.NominationListAggregate.ValueObjects;
using StarFisher.Office.Word;

namespace StarFisher.Console.Menu.SendNominationNotificationEmails
{
    public class SendNominationNotificationEmailsMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Send nomination notification emails";

        private const string SuccessMessage =
            @"Success! You should now see many nomination notification emails in your Outlook Outbox.";

        private readonly IMailMergeFactory _mailMergeFactory;

        public SendNominationNotificationEmailsMenuItemCommand(IStarFisherContext context,
            IMailMergeFactory mailMergeFactory)
            : base(context, CommandTitle, SuccessMessage)
        {
            _mailMergeFactory = mailMergeFactory ?? throw new ArgumentNullException(nameof(mailMergeFactory));
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            if (!GetReady())
                return CommandOutput.None.Abort;

            var nominationList = Context.NominationListContext.NominationList;

            foreach (var awardType in AwardType.ValidAwardTypes)
            {
                if (!nominationList.HasNominationsForAward(awardType))
                    continue;

                var mailMerge = _mailMergeFactory.GetNominationNotificationsMailMerge(awardType, nominationList);
                mailMerge.Execute();
            }

            return CommandOutput.None.Success;
        }

        private static bool GetReady()
        {
            var parameter = new ReadyParameter();
            return TryGetArgumentValue(parameter, out bool isReady) && isReady;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized && Context.NominationListContext.HasNominationListLoaded;
        }
    }
}