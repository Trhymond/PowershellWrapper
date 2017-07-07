using Powershell.Extensions;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Powershell.Connectors
{
    public abstract class Connector
    {
        private string userName = "";
        private string password = "";
        
        public Connector(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }

        public string Server { get; set; }

        public InitialSessionState SessionState { get; set; }

        public Command ConnectCommand { get; set; }

        public WSManConnectionInfo Connection { get; set; }

        protected PSCredential Credential {
            get {
                return new PSCredential(userName, password.ToSecureString());
            }
        }

        public virtual Runspace CreateRunSpace()
        {
            return Connection != null
                ? RunspaceFactory.CreateRunspace(Connection)
                : RunspaceFactory.CreateRunspace(SessionState);
        }
    }
}
