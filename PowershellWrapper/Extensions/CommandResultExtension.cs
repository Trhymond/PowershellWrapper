using Powershell.Model;
using System;

namespace Powershell.Extensions
{
    internal static class CommandResultExtension
    {
        internal static CommandResult Success<T>(this CommandResult result, T content)
        {
            return new CommandResult
            {
                Content = content,
                Status = StatusCode.SUCCESS,
                StatusText = ""
            };
        }

        internal static CommandResult Fail(this CommandResult result, Exception ex)
        {
            result = new CommandResult
            {
                Content = null,
                Status = StatusCode.FAIL,
                StatusText = "Powershell Command Execution Failed",
            };

            if (ex is PowershellException pex)
            {
                result.Errors = pex.Exceptions();
            }
            else
            {
                result.Errors = new string[] { ex.Message };
            }

            return result;
        }
    }
}
