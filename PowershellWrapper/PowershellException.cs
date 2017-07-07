using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Powershell
{
    [Serializable]
    public class PowershellException : Exception
    {
        public PowershellException(string message) : base(message)
        {

        }

        public PowershellException(string message, Dictionary<string, string[]> commandExceptions) : base(message)
        {
            CommandExceptions = commandExceptions;
        }

        public string[] ExceptionCommands()
        {
            return CommandExceptions.Select(x => x.Key).ToArray();
        }

        public string[] Exceptions(string command)
        {
            if (CommandExceptions.ContainsKey(command))
            {
               return CommandExceptions.FirstOrDefault(x => x.Key == command).Value;
            }

            return null;
        }
        public string[] Exceptions()
        {
            var result = new List<string>();
            var commands = ExceptionCommands();
            foreach (var cmd in commands)
            {
                var list = CommandExceptions.FirstOrDefault(x => x.Key == cmd);

                result.AddRange(
                    list.Value.Select(x =>
                    {
                        return $"{cmd}:{x}";
                    })
                );
            }

            return result.ToArray();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }


        private Dictionary<string, string[]> CommandExceptions {
            get;
            set;
        }
    }
}
