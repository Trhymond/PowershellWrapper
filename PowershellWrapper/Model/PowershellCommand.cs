using System.Collections.Generic;

namespace Powershell.Model
{
    public class PowershellCommand
    {
        public string Name {
            get;
            set;
        }

        public string CommandText {
            get;
            set;
        }

        public bool IsScript {
            get;
            set;
        }

        public List<PowershellCommandParameter> CommandParameters {
            get;
            set;
        }
    }
}
