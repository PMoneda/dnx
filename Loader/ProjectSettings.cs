﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using Newtonsoft.Json.Linq;
using NuGet;

namespace Loader
{
    public class ProjectSettings
    {
        public const string ProjectFileName = "project.json";

        public static bool TryGetSettings(string path, out ProjectSettings projectSettings)
        {
            projectSettings = null;

            string projectSettingsPath = Path.Combine(path, ProjectFileName);

            if (!File.Exists(projectSettingsPath))
            {
                return false;
            }

            projectSettings = new ProjectSettings();

            string json = File.ReadAllText(projectSettingsPath);
            var settings = JObject.Parse(json);
            var targetFramework = settings["targetFramework"];

            string framework = targetFramework == null ? "net40" : targetFramework.Value<string>();

            projectSettings.Name = settings["name"].Value<string>();
            projectSettings.TargetFramework = VersionUtility.ParseFrameworkName(framework);
            projectSettings.Dependencies = new List<Dependency>();
            projectSettings.ProjectFilePath = projectSettingsPath;

            if (String.IsNullOrEmpty(projectSettings.Name))
            {
                throw new InvalidDataException("A project name is required.");
            }

            var dependencies = settings["dependencies"] as JArray;
            if (dependencies != null)
            {
                foreach (JObject dependency in (IEnumerable<JToken>)dependencies)
                {
                    foreach (var prop in dependency)
                    {
                        var properties = prop.Value.Value<JObject>();

                        var version = properties["version"];

                        if (String.IsNullOrEmpty(prop.Key))
                        {
                            throw new InvalidDataException("Unable to resolve dependency ''.");
                        }

                        projectSettings.Dependencies.Add(new Dependency
                        {
                            Name = prop.Key,
                            Version = version != null ? version.Value<string>() : null
                        });
                    }
                }
            }

            return true;
        }

        public string ProjectFilePath { get; private set; }

        public string Name { get; private set; }

        public FrameworkName TargetFramework { get; private set; }

        public IList<Dependency> Dependencies { get; private set; }
    }

    public class Dependency
    {
        public string Name { get; set; }

        public string Version { get; set; }

        public override string ToString()
        {
            return Name + " " + Version;
        }
    }
}
