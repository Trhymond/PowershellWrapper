using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Powershell;
using Powershell.Connectors;
using Powershell.Model;

namespace PowershellWrapper.Samples
{
    public class ActiveDirectorySample
    {

        public static void Run()
        {
            var userName = "<AD User Name>";
            var password = "<AD Password>";

            try
            {
                // Create a connector
                var connector = new ActiveDirectoryConnector(userName, password);

                // Initialze Powershell Client
                using (var client = PowerShellClient.Create(connector))
                {
                    // Create Command
                    var geUserCommand = new PowershellCommand
                    {
                        Name = "GetUser",
                        CommandText = "Get-User",
                        CommandParameters = new List<PowershellCommandParameter> {
                                    new PowershellCommandParameter { Name = "ResultSize", Value = "unlimited" }, //"100"
                                    new PowershellCommandParameter { Name = "OrganizationalUnit", Value = "Marketing" }
                            }
                    };

                    // Execute Command 
                    var result = client.Execute<ActiveDirectory>(new List<PowershellCommand> { geUserCommand });

                    // Success, get the data
                    if (result.Status == StatusCode.SUCCESS)
                    {
                        var adUsers = result.Content;

                        Console.WriteLine($"{adUsers.Count} users found");
                    }
                    else // Failes show the errors
                    {
                        Console.WriteLine(result.Status);
                        Console.WriteLine(result.StatusText);
                        Console.WriteLine(result.Errors);                        
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class ActiveDirectory
    {
        public int Id { get; set; }
        public string SamAccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string DistinguishedName { get; set; }
        public string UserPrincipalName { get; set; }
        public string Title { get; set; }
        public string ManagerId { get; set; }
        public string ManagerName { get; set; }
        public string ManagerEmailAddress { get; set; }
        public string CompanyName { get; set; }
        public string Organization { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string PhysicalDeliveryOfficeName { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string CountryOrRegion { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobilePhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<System.DateTime> LastLogon { get; set; }
        public Nullable<System.DateTime> AccountExpires { get; set; }
        public string Status { get; set; }
        public string HomeDirectory { get; set; }
        public string HomeDrive { get; set; }
        public string Description { get; set; }
        public string EmployeeID { get; set; }
    }
}
