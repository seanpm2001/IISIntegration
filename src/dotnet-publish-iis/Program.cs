// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Extensions.CommandLineUtils;

namespace Microsoft.AspNetCore.Tools.PublishIIS
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "dotnet publish-iis",
                FullName = "Asp.Net IIS Publisher",
                Description = "IIS Publisher for the Asp.Net web applications",
            };
            app.HelpOption("-h|--help");

            var publishFolderOption = app.Option("--publish-folder|-p", "The path to the publish output folder", CommandOptionType.SingleValue);
            var webRootOption = app.Option("--webroot|-w", "The name of webroot folder", CommandOptionType.SingleValue);
            var projectPath = app.Argument("<PROJECT>", "The path to the project (project folder or project.json) being published. If empty the current directory is used.");

            app.OnExecute(() =>
            {
                var publishFolder = publishFolderOption.Value();

                if (publishFolder == null)
                {
                    app.ShowHelp();
                    return 2;
                }

               return new PublishIISCommand(publishFolder, projectPath.Value, webRootOption.Value()).Run();
            });

            try
            {
                return app.Execute(args);
            }
            catch (Exception e)
            {
#if DEBUG
                Console.Error.WriteLine(e);
#else
                Console.Error.WriteLine(e.Message);
#endif
            }

            return 1;
        }
    }
}