using Powershell.Connectors;
using Powershell.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Powershell.Extensions
{
    internal static class ConnectorExtension
    {
        internal static Collection<PSObject> ExecuteCommand(this Connector connector, List<Command> commands)
        {
            var errors = new Dictionary<string, string[]>();
            var results = new Collection<PSObject>();

            if (connector.ConnectCommand != null)
            {
                commands.Insert(0, connector.ConnectCommand);
            }

            using (var psRunSpace = connector.CreateRunSpace())
            {
                // Open runspace.
                psRunSpace.Open();

                foreach (var command in commands)
                {
                    var pipe = psRunSpace.CreatePipeline();

                    if (command.IsScript)
                    {
                        pipe.Commands.AddScript(command.CommandText);
                    }
                    else
                    {
                        pipe.Commands.Add(command);
                    }

                    // Execute command and generate results and errors (if any).
                    results = pipe.Invoke();

                    var error = pipe.Error.ReadToEnd();
                    if (error.Count > 0)
                    {
                        errors.Add(command.CommandText, error.Select(x => x.ToString()).ToArray());
                    }
                }

                // psRunSpace.Close();                
            }

            if (errors.Count > 0)
            {
                throw new PowershellException("Some error occured while executing the powershell commands.", errors);
            }

            return results;
        }

        internal static Collection<PSObject> ExecuteCommand(this Connector connector, Dictionary<string, Dictionary<string, object>> commands)
        {
            var psCommands = new List<Command>();

            foreach (var cmd in commands.Keys)
            {
                var command = new Command(cmd);
                var commandParameters = commands[cmd];
                foreach (var param in commandParameters.Keys)
                {
                    command.Parameters.Add(new CommandParameter(param, commandParameters[param]));
                }

                psCommands.Add(command);
            }

            return connector.ExecuteCommand(psCommands);
        }

        internal static Collection<PSObject> ExecuteCommand(this Connector connector, string commandText, Dictionary<string, object> commandParameters)
        {
            var command = new Command(commandText);
            foreach (var param in commandParameters.Keys)
            {
                command.Parameters.Add(new CommandParameter(param, commandParameters[param]));
            }

            return connector.ExecuteCommand(new List<Command> { command });
        }

        internal static Collection<PSObject> ExecuteCommand(this Connector connector, Command command)
        {
            return connector.ExecuteCommand(new List<Command> { command });
        }

        internal static Collection<PSObject> ExecuteRemoteCommand(this Connector connector, List<PowershellCommand> commands)
        {
            using (var psRunSpace = connector.CreateRunSpace()) {
                using (var powershell = PowerShell.Create())
                {
                    powershell.Commands.Clear();

                    foreach (var cmd in commands)
                    {
                        if (cmd.IsScript)
                        {
                            powershell.AddScript(cmd.CommandText);
                        }
                        else
                        {
                            powershell.AddCommand(cmd.CommandText);
                            foreach (var param in cmd.CommandParameters)
                            {
                                if (param.IsArgument)
                                {
                                    powershell.AddArgument(param.Value);
                                }
                                else
                                {
                                    powershell.AddParameter(param.Name, param.Value);
                                }
                            }
                        }
                    }

                    powershell.Runspace = psRunSpace;
                    return powershell.Invoke();
                }
            }
        }

        //internal static PSObject ExecuteCommadX(this Connector connector, Command command)
        //{
        //    PSObject result = null;
        //    using (Runspace psRunSpace = RunspaceFactory.CreateRunspace(connector.SessionState))
        //    {
        //        // Open runspace.
        //        psRunSpace.Open();

        //        var pipe = psRunSpace.CreatePipeline();
        //        pipe.Commands.Add(command);

        //        // Execute command and generate results and errors (if any).
        //        var results = pipe.Invoke();

        //        result = results[0];

        //        psRunSpace.Close();
        //    }

        //    return result;
        //}

        internal static Command ToPSCommand(this PowershellCommand command)
        {
            var psCommand = new Command(command.CommandText);
            if (command.CommandParameters != null)
            {
                foreach (var parm in command.CommandParameters)
                {
                    psCommand.Parameters.Add(new CommandParameter(parm.Name, parm.Value));
                }
            }

            return psCommand;
        }
    }
}
