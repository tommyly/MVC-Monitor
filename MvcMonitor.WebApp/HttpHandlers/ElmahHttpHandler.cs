﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcMonitor.ErrorHandling;
using MvcMonitor.Models.Factories;

namespace MvcMonitor.HttpHandlers
{
    public class ElmahHttpHandler : BaseHttpHandler
    {
        private readonly IErrorProcessor _errorProcessor;
        private readonly IElmahErrorDtoFactory _elmahErrorDtoFactory;
        private readonly List<string> _applications;

        public ElmahHttpHandler()
            : this(new ErrorProcessor(), new ElmahErrorDtoFactory(), MonitorConfiguration.Applications)
        {
        }

        public ElmahHttpHandler(IErrorProcessor errorProcessor, IElmahErrorDtoFactory elmahErrorDtoFactory, List<string> applications)
        {
            _errorProcessor = errorProcessor;
            _elmahErrorDtoFactory = elmahErrorDtoFactory;
            _applications = applications;
        }

        public override void ProcessRequest(HttpContextBase context)
        {
            try
            {
                var errorId = context.Request.Params["errorId"];
                var sourceApplication = context.Request.Params["sourceId"];
                var infoUrl = context.Request.Params["infoUrl"];
                var errorDetails = context.Request.Params["error"];

                if (!_applications.Any(app => app.Equals(sourceApplication, StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                var errorDto = _elmahErrorDtoFactory.Create(errorId, sourceApplication, infoUrl, errorDetails);

                _errorProcessor.ProcessElmahError(errorDto);    

                Logger.Log.DebugFormat("Completed processing error {0} for {1}", errorId, sourceApplication);
            }
            catch (Exception exc)
            {
                Logger.Log.Error("There was an error processing the request", exc);
                throw;
            }
        }
    }
}