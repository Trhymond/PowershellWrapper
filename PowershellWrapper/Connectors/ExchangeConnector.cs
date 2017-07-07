using System;
using System.Management.Automation.Runspaces;

namespace Powershell.Connectors
{
    public class ExchangeConnector : Connector
    {
        private string schemaUrl = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

        public ExchangeConnector(string server, string userName, string password) 
            : base(userName, password)
        {
            Server = server;
            InitializeConnector();
        }

        private void InitializeConnector()
        {
            var url = $"{Server}/PowerShell";

            Connection = new WSManConnectionInfo(new Uri(url), schemaUrl, Credential)
            {
                // AuthenticationMechanism = AuthenticationMechanism.Kerberos,
                AuthenticationMechanism = AuthenticationMechanism.Basic,
                EnableNetworkAccess = true,
                MaximumConnectionRedirectionCount = 1,

                SkipCACheck = true,
                SkipCNCheck = true
            };
        }
    }
}
