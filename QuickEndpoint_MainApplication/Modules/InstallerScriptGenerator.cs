using System;
using System.IO;
using QuickEndpoint;

namespace QuickEndpoint.Modules
{
	public class InstallerScriptGenerator
	{
		private string tempPublishDir = Path.Combine(@"E:\source_code\QuickEndpoint", "tempPublish");
		private string projectName = "QuickEndpoint_ApiExample";
		private string nssmPath = @"E:\source_code\QuickEndpoint\Tools\nssm-2.24\win64\nssm.exe";

		public void GenerateInstallScript()
		{
			Console.WriteLine("Generating installer script...");
			string archivePath = Path.Combine(@"E:\source_code\QuickEndpoint\Publish", $"{projectName}.tar.gz");
			string scriptTemplate = ScriptTemplateGenerator.GetScriptTemplate(projectName, archivePath, $"{projectName}.exe", nssmPath);
			string tempScriptPath = Path.Combine(tempPublishDir, $"{projectName}_install.ps1");
			File.WriteAllText(tempScriptPath, scriptTemplate);

			Console.WriteLine($"Installer script has been temporarily saved at: {tempScriptPath}");
		}
	}
}
