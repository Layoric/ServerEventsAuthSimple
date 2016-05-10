using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using Funq;
using AngularJSApp9.ServiceInterface;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Razor;

namespace AngularJSApp9
{
    public class AppHost : AppHostBase
    {
        /// <summary>
        /// Default constructor.
        /// Base constructor requires a name and assembly to locate web service classes. 
        /// </summary>
        public AppHost()
            : base("AngularJSApp9", typeof(MyServices).Assembly)
        {
            var customSettings = new FileInfo(@"~/appsettings.txt".MapHostAbsolutePath());
            AppSettings = customSettings.Exists
                ? (IAppSettings)new TextFileSettings(customSettings.FullName)
                : new AppSettings();
        }

        /// <summary>
        /// Application specific configuration
        /// This method should initialize any IoC resources utilized by your web service classes.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {
            //Config examples
            //this.Plugins.Add(new PostmanFeature());
            //this.Plugins.Add(new CorsFeature());

            SetConfig(new HostConfig
            {
                DebugMode = AppSettings.Get("DebugMode", false),
                AddRedirectParamsToQueryString = true
            });

            this.Plugins.Add(new RazorFormat());

            Plugins.Add(new AuthFeature(
               () => new AuthUserSession(), //Use your own typed Custom UserSession type
               new IAuthProvider[] {
                    new CredentialsAuthProvider()              //HTML Form post of UserName/Password credentials
               }));

            //Provide service for new users to register so they can login with supplied credentials.
            Plugins.Add(new RegistrationFeature());

            var authRepo = new InMemoryAuthRepository();
            container.Register<IUserAuthRepository>(authRepo);
            authRepo.CreateUserAuth(new UserAuth { UserName = "Test", DisplayName = "FooBar" }, "testtest");
            authRepo.CreateUserAuth(new UserAuth { UserName = "Test2", DisplayName = "FooBar2" }, "testtest");
            authRepo.CreateUserAuth(new UserAuth { UserName = "Test3", DisplayName = "FooBar3" }, "testtest");
            authRepo.CreateUserAuth(new UserAuth { UserName = "Test4", DisplayName = "FooBar4" }, "testtest");
            authRepo.CreateUserAuth(new UserAuth { UserName = "Test5", DisplayName = "FooBar5" }, "testtest");

            this.Plugins.Add(new AuthFeature(() => new AuthUserSession(), new IAuthProvider[]
            {
                new CredentialsAuthProvider(new AppSettings()),
            },"/"));

            Plugins.Add(new ServerEventsFeature
            {
                StreamPath = "/event-stream",
                HeartbeatPath = "/event-heartbeat",
                UnRegisterPath = "/event-unregister",
                SubscribersPath = null,
                LimitToAuthenticatedUsers = true,
                NotifyChannelOfSubscriptions = false
            });

        }
    }
}