using System;
using QuickEndpoint.Modules;

namespace QuickEndpoint
{
    class Program
    {
        static void Main(string[] args)
        {
            var publisher = new ApplicationPublisher();
            publisher.PublishApplication();

            var scriptGenerator = new InstallerScriptGenerator();
            scriptGenerator.GenerateInstallScript();

            var packer = new ApplicationPacker();
            packer.PackFinalApplication();
        }
    }
}