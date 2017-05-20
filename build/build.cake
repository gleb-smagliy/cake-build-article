#tool "nuget:?package=NUnit.ConsoleRunner&version=3.6.0"

var configuration = Argument("configuration", "Debug");
var nugetApiKey = Argument("NugetApiKey", "");
var publishRemotely = Argument("PublishRemotely", false);

Task("Build")
	.Does(() =>
{
	NuGetRestore("../Solution/Solution.sln");
	DotNetBuild("../Solution/Solution.sln", x => x
        .SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal)
        .WithTarget("build")
        .WithProperty("TreatWarningsAsErrors", "false")
    );
});

Task("Tests::Unit")
.IsDependentOn("Build")
.Does(()=> 
{
	NUnit3(@"..\Solution\MyProject.Tests\bin\" + configuration + @"\MyProject.Tests.dll");
});

Task("Pack")
.IsDependentOn("Tests::Unit")
.Does(()=> 
{
	var packageDir = @"..\package";
	var artefactsDir = @"..\.artefacts";

	MoveFiles("*.nupkg", packageDir);

	EnsureDirectoryExists(packageDir);
	CleanDirectory(packageDir);

	EnsureDirectoryExists(artefactsDir);
	CleanDirectory(artefactsDir);
	CopyFiles(@"..\Solution\MyProject\bin\" + configuration + @"\*.dll", artefactsDir);
	CopyFiles(@"..\Solution\MyProject\bin\" + configuration + @"\*.pdb", artefactsDir);
	CopyFileToDirectory(@".\Solution.nuspec", artefactsDir);

	NuGetPack(new FilePath(artefactsDir + @"\Solution.nuspec"), new NuGetPackSettings
	{
		OutputDirectory = packageDir
	});
});

Task("Publish")
.IsDependentOn("Pack")
.WithCriteria(publishRemotely)
.Does(()=> 
{
	NuGetPush(GetFiles(@"..\package\*.nupkg").First(), new NuGetPushSettings {
    	Source = "https://www.nuget.org/api/v2",
    	ApiKey = nugetApiKey
 	});
});

Task("BuildAndPublish")
.IsDependentOn("Publish")
.Does(()=> 
{
});

RunTarget("BuildAndPublish");