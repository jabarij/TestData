var target = Argument("target", "Build");
var configuration = Argument("Configuration", "Release");
var dotNetCoreVerbosity = (DotNetCoreVerbosity)Enum.Parse(typeof(DotNetCoreVerbosity), Argument("DotNetCoreVerbosity", "Normal"));

var outputRootDir = "./build/";
var buildOutputDir = outputRootDir + "artifacts/";
var testsOutputDir = outputRootDir + "tests/";
var nugetOutputDir = outputRootDir + "nuget/";

var sourceRootDir = "./src/";
var solutionPath_TestData = sourceRootDir + "TestData.sln";
var projectPath_TestData = sourceRootDir + "TestData/TestData.csproj";

GitVersion version = null;

var resolveVersionTask = Task("Resolve-Version")
  .Does(() =>
  {
		version = GitVersion(new GitVersionSettings {
			UpdateAssemblyInfo = true
		});    
    Information($"GitVersion.FullSemVer: {version.FullSemVer}");
  });

var buildSolutionTask = Task("Build-Solution")
  .IsDependentOn(resolveVersionTask)
  .Does(() =>
  {
    DotNetCoreBuild(solutionPath_TestData,
      new DotNetCoreBuildSettings
      {
        Configuration = configuration,
        OutputDirectory = buildOutputDir,
        ArgumentCustomization = args => args
          .Append($"-p:PackageVersion={version.FullSemVer}")
      });
  });
  
var runUnitTestsTask = Task("Run-UnitTests")
  .IsDependentOn(buildSolutionTask)
  .Does(() =>
  {
    var testProjectFiles = GetFiles(sourceRootDir + "**/*.Tests.csproj");
    foreach(var project in testProjectFiles)
    {
      Information("Testing project: " + project);
      DotNetCoreTest(project.FullPath,
        new DotNetCoreTestSettings
        {
          Configuration = configuration,
          NoBuild = true,
          OutputDirectory = buildOutputDir,
          ResultsDirectory = testsOutputDir,
          Verbosity = dotNetCoreVerbosity,          
          ArgumentCustomization = args => args
            //.Append($"--results-directory \"" + testsOutputDir + "\"")
            .Append($"--logger \"trx;LogFileName=testResults.trx\"")
        });
    }
  });

var buildTask = Task("Build")
  .IsDependentOn(buildSolutionTask)
  .IsDependentOn(runUnitTestsTask)
  .Does(() =>
  {
  });
  
var publishNuGetPackageTask = Task("Publish-NuGetPackage")
  .Does(() =>
  {
  });

var publishTask = Task("Publish")
  .IsDependentOn(buildTask)
  .IsDependentOn(publishNuGetPackageTask)
  .Does(() =>
  {
  });

RunTarget(target);