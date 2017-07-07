namespace Powershell.Model
{
    public class CommandResult 
    {
        public object Content { get; set; }

        public StatusCode Status { get; set; }

        public string StatusText { get; set; }
        public string[] Errors { get; set; }

    }
}
