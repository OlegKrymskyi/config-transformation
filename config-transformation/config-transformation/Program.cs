using DotLiquid;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace config_transformation
{
    class Program
    {
        private static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
              .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("app.json", optional: true, reloadOnChange: false)
              .AddEnvironmentVariables()
              .AddCommandLine(args);

            configuration = builder.Build();

            if (args.Length == 0)
            {
                Console.WriteLine("The path to the folder with the templates which must be processed is not specified");
            }

            if (!Directory.Exists(args[0])) 
            {
                Console.WriteLine("Invalid path to the templates which must be processed");
            }

            var configObj = GetConfigurationObject(configuration);

            foreach (var file in Directory.GetFiles(args[0], "*.template", SearchOption.AllDirectories))
            {
                var template = Template.Parse(File.ReadAllText(file));

                var output = template.Render(Hash.FromAnonymousObject(configObj));
                using (var fs = new FileStream(Path.GetFileNameWithoutExtension(file), FileMode.Create, FileAccess.Write))
                {
                    using (var writter = new StreamWriter(fs))
                    {
                        writter.Write(output);
                    }
                }
            }
        }

        private static dynamic GetConfigurationObject(IConfiguration config)
        {
            var result = new ExpandoObject();

            // retrieve all keys from your settings
            foreach (var kvp in config.AsEnumerable())
            {
                var parent = result as IDictionary<string, object>;
                var path = kvp.Key.Split(':');

                // create or retrieve the hierarchy (keep last path item for later)
                var i = 0;
                for (i = 0; i < path.Length - 1; i++)
                {
                    if (!parent.ContainsKey(path[i]))
                    {
                        parent.Add(path[i], new ExpandoObject());
                    }

                    parent = parent[path[i]] as IDictionary<string, object>;
                }

                if (kvp.Value == null)
                    continue;

                // add the value to the parent
                // note: in case of an array, key will be an integer and will be dealt with later
                var key = path[i];
                parent.Add(key, kvp.Value);
            }

            return result;
        }
    }
}
