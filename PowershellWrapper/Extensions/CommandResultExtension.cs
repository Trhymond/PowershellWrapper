using Powershell.Model;
using System;
using System.Collections.Generic;

namespace Powershell.Extensions
{
    internal static class CommandResultExtension
    {
        internal static CommandResult<T> Success<T>(this CommandResult<T> result, List<T> content)
        {
            return new CommandResult<T>
            {
                Content = content,
                Status = StatusCode.SUCCESS,
                StatusText = ""
            };
        }

        internal static CommandResult<T> Fail<T>(this CommandResult<T> result, Exception ex)
        {
            result = new CommandResult<T>
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
