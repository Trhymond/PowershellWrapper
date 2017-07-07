using System.Collections.Generic;

namespace Powershell.Model
{
    public class CommandResult<T>
    {
        public List<T> Content { get; set; }

        public StatusCode Status { get; set; }

        public string StatusText { get; set; }
        public string[] Errors { get; set; }

    }
}
