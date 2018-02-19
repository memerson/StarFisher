using StarFisher.Console.Menu.Common;
using StarFisher.Domain.ValueObjects;

namespace StarFisher.Console.Menu.Initialize.Parameters
{
    public class WorkingDirectoryParameter : ParameterBase<DirectoryPath>
    {
        private static readonly DirectoryPath DefaultWorkingDirectoryPath = DirectoryPath.Create(@"C:\EIA\StarFisher");

        public WorkingDirectoryParameter()
        {
            RegisterValidInput(@"default", DefaultWorkingDirectoryPath);
            RegisterAbortInput(@"stop");
        }

        public override Argument<DirectoryPath> GetArgumentCore()
        {
            WriteLine();
            WriteCallToAction(
                $@"Enter the full path of a directory you want to use as your working directory for StarFisher. Alternatively, you can enter 'default' to use the default directory of {
                        DefaultWorkingDirectoryPath.Value
                    }. You can also enter 'stop' to stop the initialization workflow.");
            WriteInputPrompt();

            return GetArgumentFromInputIfValid();
        }

        protected override bool TryParseArgumentValueFromInput(string input, out DirectoryPath argumentValue)
        {
            if (DirectoryPath.IsValid(input))
            {
                argumentValue = DirectoryPath.Create(input);
                return true;
            }

            argumentValue = null;
            return false;
        }

        public override void PrintInvalidArgumentMessage()
        {
            PrintInvalidArgumentMessage(@"That's not a valid directory.");
        }
    }
}