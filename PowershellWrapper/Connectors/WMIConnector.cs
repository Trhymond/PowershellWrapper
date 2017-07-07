using System.Management.Automation.Runspaces;

namespace Powershell.Connectors
{
    public class WMIConnector : Connector
    {
        public WMIConnector(string userName, string password) 
            : base(userName, password)
        {
            InitializeConnector();
        }

        private void InitializeConnector()
        {
            SessionState = InitialSessionState.CreateDefault();
            // SessionState.ExecutionPolicy = Microsoft.PowerShell.ExecutionPolicy.Unrestricted;
        }
    }
}
