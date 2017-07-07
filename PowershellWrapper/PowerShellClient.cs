using Powershell.Connectors;
using Powershell.Extensions;
using Powershell.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;

namespace Powershell
{
    /// <summary>
    /// PowerShellClient
    /// </summary>
    /// <seealso cref="Powershell.IClient" />
    public class PowerShellClient : IClient
    {
        private bool disposed = false;
        private Connector psConnector;

        private PowerShellClient(Connector connector)
        {
            psConnector = connector;
        }

        /// <summary>
        /// Creates the specified connector.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <returns></returns>
        public static PowerShellClient Create(Connector connector)
        {
            return new PowerShellClient(connector);
        }


        /// <summary>
        /// Executes the specified commands.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commands">The commands.</param>
        /// <param name="childCommands">The child commands.</param>
        /// <returns></returns>
        public CommandResult Execute<T>(List<PowershellCommand> commands, List<PowershellCommand> childCommands = null)
        {
            try
            {
                var instance = (IList)typeof(List<>)
                    .MakeGenericType(typeof(T))
                    .GetConstructor(Type.EmptyTypes)
                    .Invoke(null);

                var psCommands = commands.ToDictionary(x => x.CommandText, y => y.CommandParameters.ToDictionary(a => a.Name, b => b.Value));
                var results = psConnector.ExecuteCommand(psCommands);
                if (results != null && results.Count > 0)
                {
                    foreach (PSObject obj in results)
                    {
                        var objT = (T)obj.PSConvert<T>();

                        if (childCommands != null)
                        {
                            foreach (var pcmd in childCommands)
                            {
                                foreach (var p in pcmd.CommandParameters.Where(x => x.Value == null))
                                {
                                    p.Value = obj.Properties[p.Name].Value;
                                }

                                var childResults = psConnector.ExecuteCommand(new List<Command> { pcmd.ToPSCommand() });
                                objT.UpdateObjectProperties<T>(childResults, false);
                            }
                        }

                        instance.Add(objT);
                    }
                }
                return new CommandResult()
                                   .Success<IList<T>>((List<T>)instance);
            }
            catch (Exception ex)
            {
                return new CommandResult()
                                   .Fail(ex);
            }                       
        }

        /// <summary>
        /// Executes the specified command and child commands on remote server.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commands">The commands.</param>
        /// <param name="childCommands">The child commands.</param>
        /// <returns></returns>
        public CommandResult ExecuteRemote<T>(List<PowershellCommand> commands, List<PowershellCommand> childCommands = null)
        {
            Runspace runspace = null;

            try
            {
                var instance = (IList)typeof(List<>)
                    .MakeGenericType(typeof(T))
                    .GetConstructor(Type.EmptyTypes)
                    .Invoke(null);

                runspace = psConnector.CreateRunSpace();

                runspace.Open();
                var results = psConnector.ExecuteRemoteCommand(commands);
                if (results != null && results.Count > 0)
                {
                    foreach (PSObject obj in results)
                    {
                        var objT = (T)obj.PSConvert<T>();

                        if (childCommands != null)
                        {
                            foreach (var pcmd in childCommands)
                            {
                                foreach (var p in pcmd.CommandParameters.Where(x => x.Value == null))
                                {
                                    p.Value = obj.Properties[p.Name].Value;
                                }

                                var childResults = psConnector.ExecuteRemoteCommand(new List<PowershellCommand> { pcmd });
                                objT.UpdateObjectProperties<T>(childResults, false);
                            }
                        }
                        instance.Add(objT);
                    }
                }

                return new CommandResult()
                                   .Success<IList<T>>((List<T>)instance);

            }
            catch (Exception ex)
            {
                return new CommandResult()
                                   .Fail(ex);
            }
            finally
            {
                if (runspace != null)
                {
                    runspace.Dispose();
                }
            }
        }


        /// <summary>
        /// Executes the specified commands.
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <returns></returns>
        public CommandResult ExecuteNonQuery(List<PowershellCommand> commands)
        {
            try
            {
                var psCommands = commands.ToDictionary(x => x.CommandText, y => y.CommandParameters.ToDictionary(a => a.Name, b => b.Value));
                var results = psConnector.ExecuteCommand(psCommands);

                var stringBuilder = new StringBuilder();
                foreach (PSObject obj in results)
                {
                    stringBuilder.AppendLine(obj.ToString());
                }

                return new CommandResult()
                                   .Success<string>(stringBuilder.ToString());
            }
            catch (Exception ex)
            {
                return new CommandResult()
                                 .Fail(ex);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                
            }

            disposed = true;
        }
    }
}
