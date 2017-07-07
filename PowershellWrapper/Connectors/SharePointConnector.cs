using System.Management.Automation.Runspaces;

namespace Powershell.Connectors
{
    public class SharePointConnector : Connector
    {
        public SharePointConnector(string server, string userName, string password) 
            : base(userName, password)
        {
            Server = server;
            InitializeConnector();
        }

        private void InitializeConnector()
        {
            SessionState = InitialSessionState.CreateDefault();
            // SessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;

            SessionState.ImportPSModule(new[] { "Microsoft.Online.SharePoint.PowerShell" });

            ConnectCommand = new Command("Connect-SPOService");
            ConnectCommand.Parameters.Add("Url", Server);
            ConnectCommand.Parameters.Add("Credential", Credential);
        }
    }
}
