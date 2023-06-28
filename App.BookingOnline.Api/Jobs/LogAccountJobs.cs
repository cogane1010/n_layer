using App.BookingOnline.Service;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Threading.Tasks;

namespace App.BookingOnline.WebApi.Jobs
{
    public class LogAccountJobs
    {
        private readonly ILogger _log;
        public LogAccountJobs(ILogger<LogAccountJobs> logger)
        {
            _log = logger;
        }

        public async Task logAccountCustomer()
        {
            try
            {
                //_service.LockAccountCustomerDueNoShow();
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                    _log.LogError(e.StackTrace);
                }
            }
        }
    }
}
