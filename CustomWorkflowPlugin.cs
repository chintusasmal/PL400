using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;

namespace WorkFlowActivity
{
    public class ConvertFormatDateTime : CodeActivity
    {
        [RequiredArgument]
        [Input("DateTime input")]
        public InArgument<DateTime> DateToEvaluate { get; set; }
        [Output("Formatted DateTime output as string")]
        public OutArgument<String> FormattedDateTimeOutput { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IWorkflowContext workflowContext = context.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = serviceFactory.CreateOrganizationService(workflowContext.UserId);

            //Get the input datetime. Add the following snippet to the Execute method.
            DateTime utcDateTime = this.DateToEvaluate.Get(context);

            //Check if the input datetime is UTC, if not, convert it to UTC. Add the following snippet to the Execute method.
            if (utcDateTime.Kind != DateTimeKind.Utc)
            {
                utcDateTime = utcDateTime.ToUniversalTime();
            }

            //Get user settings and convert datetime to user local datetime

            var settings = service.Retrieve("usersettings", workflowContext.UserId, new ColumnSet("timezonecode"));

            //Build a time zone change request by providing the Utc time that you got from the input and the TimeZoneCode from the user settings

            LocalTimeFromUtcTimeRequest timeZoneChangeRequest = new LocalTimeFromUtcTimeRequest()
            {
                UtcTime = utcDateTime,
                TimeZoneCode = int.Parse(settings["timezonecode"].ToString())
            };

            //Run the time zone change request.
            LocalTimeFromUtcTimeResponse timeZoneResponse = service.Execute(timeZoneChangeRequest) as LocalTimeFromUtcTimeResponse;
            
            //Format the LocalTime from the time zone response and set it to the output of the activity.
            this.FormattedDateTimeOutput.Set(context, String.Format("{0:f}",timeZoneResponse.LocalTime));
        
        }
    }
}
