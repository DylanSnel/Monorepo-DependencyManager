using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace Slyng.Monorepo.DependencyManager.Models
{
    public class BuildFile
    {

        public BuildFile(string path, string projectDirectory)
        {
            FullPath = path;
            ProjectDirectory = projectDirectory;
        }
        public string FullPath { get; set; }
        public string ProjectDirectory { get; }

        public void SetDependencies(List<string> depenciesPaths)
        {
            depenciesPaths.Insert(0, ProjectDirectory.Replace("\\", "/").TrimEnd('/') + "/*");

            YamlStream yaml = new YamlStream();
            using (var sr = new StreamReader(FullPath))
            {
                yaml.Load(sr);
            }
            var mapping =
                (YamlMappingNode)yaml.Documents[0].RootNode;
            var trigger = (YamlMappingNode)mapping.Children[new YamlScalarNode("trigger")];
            var paths = (YamlMappingNode)trigger.Children[new YamlScalarNode("paths")];
            var include = (YamlSequenceNode)paths.Children[new YamlScalarNode("include")];
            include.Children.Clear();
            foreach (var dependency in depenciesPaths)
            {
                include.Children.Add(new YamlScalarNode(dependency));
            }
            var buffer = new StringBuilder();
            var yamlStream = new YamlStream(yaml.Documents[0]);

            using (var writer = new StringWriter(buffer))
            {
                yamlStream.Save(writer, false);
            }
            using (TextWriter writer = File.CreateText(FullPath))
            {
                yamlStream.Save(writer, false);
            }
        }
    }
}
