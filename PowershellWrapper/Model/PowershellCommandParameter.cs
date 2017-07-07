namespace Powershell.Model
{
    public class PowershellCommandParameter
    {
        public string Name {
            get;
            set;
        }

        public object Value {
            get;
            set;
        }

        public bool IsArgument {
            get;
            set;
        }
    }
}
