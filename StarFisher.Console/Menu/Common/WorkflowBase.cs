//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace StarFisher.Console.Commands2.Common
//{
//    public abstract class WorkflowBase : ICommand
//    {
//        private readonly ICollection<ICommand> _commands;

//        protected WorkflowBase(string title, ICollection<ICommand> commands)
//        {
//            Title = title;
//            _commands = commands;
//        }

//        public string Title { get; }

//        public CommandResultType Run()
//        {
//            foreach (var command in _commands)
//            {
//                var result = command.Run();

//                switch (result)
//                {
//                        case CommandResultType.Success:
//                            continue;
//                }
//            }
//        }
//    }
//}
