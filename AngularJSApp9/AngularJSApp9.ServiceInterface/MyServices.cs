using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack;
using AngularJSApp9.ServiceModel;

namespace AngularJSApp9.ServiceInterface
{
    public class MyServices : Service
    {
        public IServerEvents ServerEvents { get; set; }

        public object Any(Hello request)
        {
            ServerEvents.NotifyChannel("MyChannel", "cmd.showNotification", new {Message = "testing"});
            return new HelloResponse { Result = "Hello, {0}!".Fmt(request.Name) };
        }
    }
}