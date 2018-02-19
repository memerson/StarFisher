using StarFisher.Console.Context;
using StarFisher.Console.Menu.Common;
using StarFisher.Console.Menu.LoadNominationsFromSurveyExport.Parameters;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.LoadNominationsFromSurveyExport
{
    public class LoadNominationsFromSurveyExportMenuItemCommand : MenuItemCommandBase
    {
        private const string CommandTitle = @"Load nominations from survey export";

        public LoadNominationsFromSurveyExportMenuItemCommand(IStarFisherContext context) : base(context, CommandTitle)
        {
        }

        protected override CommandResult<CommandOutput.None> RunCore(CommandInput.None input)
        {
            var parameter = new SurveyExportFilePathParameter();
            if (!TryGetArgumentValue(parameter, out FilePath filePath))
                return CommandOutput.None.Abort;

            Context.NominationListContext.LoadSurveyExport(filePath);
            Context.NominationListContext.SaveSnapshot();

            return CommandOutput.None.Success;
        }

        public override bool GetCanRun()
        {
            return Context.IsInitialized;
        }
    }
}