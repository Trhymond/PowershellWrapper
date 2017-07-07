using System.Management.Automation.Runspaces;

namespace Powershell.Connectors
{
    public class MsOnlineConnector : Connector
    {
        public MsOnlineConnector(string userName, string password) 
            : base(userName, password)
        {
            InitializeConnector();
        }

        private void InitializeConnector()
        {
            SessionState = InitialSessionState.CreateDefault();
            SessionState.ImportPSModule(new[] { "MSOnline" });

            ConnectCommand = new Command("Connect-MsolService");
            ConnectCommand.Parameters.Add("Credential", Credential);
        }
    }
}
