using System.IO;

namespace Powershell.Connectors
{
    public class FileConnector : Connector
    {
        
        public FileConnector(string scriptText) : base(null, null)
        {
            ScriptText = scriptText;
        }

        public FileConnector(FileStream stream) : base(null, null)
        {
            ScriptText = new StreamReader(stream).ReadToEnd();
        }

        private void InitializeConnector(FileStream stream)
        {
        }

        public string ScriptText {
            get;
            private set;
        }
    }
}
