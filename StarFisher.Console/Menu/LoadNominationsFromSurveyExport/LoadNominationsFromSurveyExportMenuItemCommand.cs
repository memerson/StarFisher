using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.LoadNominationsFromSurveyExport.Parameters;

namespace StarFisher.Console.Menu.LoadNominationsFromSurveyExport
{
    public class LoadNominationsFromSurveyExportMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Load nominations from survey export";

        public LoadNominationsFromSurveyExportMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle) { }

        public LoadNominationsFromSurveyExportMenuItemCommand() : base(CommandTitle) { }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var parameter = new SurveyExportFilePathParameter();
            var argument = parameter.GetValidArgument();

            if (argument.ArgumentType == ArgumentType.Abort)
                return CommandOutput.None.Abort;

            Context.NominationListContext.LoadSurveyExport(argument.Value);
            Context.NominationListContext.SaveSnapshot();

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized;
        }
    }
}
