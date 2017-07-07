using Powershell.Model;
using System;
using System.Collections.Generic;

namespace Powershell
{
    public interface IClient : IDisposable
    {
        CommandResult Execute<T>(List<PowershellCommand> commands, List<PowershellCommand> childCommands = null);
        CommandResult ExecuteRemote<T>(List<PowershellCommand> commands, List<PowershellCommand> childCommands = null);
        CommandResult ExecuteNonQuery(List<PowershellCommand> commands);

    }
}
