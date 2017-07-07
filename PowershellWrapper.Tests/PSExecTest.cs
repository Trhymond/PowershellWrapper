using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Powershell.Connectors;
using System.IO;
using Powershell.Model;
using System.Collections.Generic;
using Powershell;

namespace PowershellWrapper.Tests
{
    [TestClass]
    public class PSExecTest
    {
        [TestMethod]
        public void TestExecutePSScriptSuccess()
        {
            var scriptFile = "";
            var connector = new FileConnector(new FileStream(scriptFile, FileMode.Open));
            using (var client = PowerShellClient.Create(connector))
            {
                var result = client.ExecuteNonQuery(
                    new List<PowershellCommand>
                    {
                        new PowershellCommand {
                            IsScript = true, CommandText = connector.ScriptText
                        }
                    });

                Assert.Equals(result.Status, StatusCode.SUCCESS);
            }
        }
    }
}
