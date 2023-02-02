using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.Text.RegularExpressions;


    namespace D365PackageProject
{
    public class PreOperationFormatPhoneCreateUpdate : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            
            //Check the input parameter for Target. Add the following snippet to the Execute method
            if (!context.InputParameters.ContainsKey("Target"))
                throw new InvalidPluginExecutionException("No target found");

            //This snippet will get the target entity from the input parameter and then check if its attributes contain telephone1 (Business Phone for Contacts, Phone for Accounts).
            var entity = context.InputParameters["Target"] as Entity;
            if (!entity.Attributes.Contains("telephone1"))
                return;

            //This snippet will remove all nonnumeric characters from the user-provided phone number.
            string phoneNumber = (string)entity["telephone1"];
            var formattedNumber = Regex.Replace(phoneNumber, @"[^\d]", "");

            //Set telephone1 to the formatted phone number
            entity["telephone1"] = formattedNumber;
        }
    }
}