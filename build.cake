var target = Argument("target", "Build");
var configuration = Argument("Configuration", "Release");
DotNetCoreVerbosity dotNetCoreVerbosity = (DotNetCoreVerbosity)Enum.Parse(typeof(DotNetCoreVerbosity), Argument("DotNetCoreVerbosity", "Normal"));
string nugetApiKey = Argument("NuGetApiKey", "");
string nugetPushSource = Argument("NuGetPushSource", "https://api.nuget.org/v3/index.json");

var outputRootDir = "./build/";
var buildOutputDir = outputRootDir + "artifacts/";
var testsOutputDir = outputRootDir + "tests/";

var sourceRootDir = "./src/";
var solutionPath_TestData = sourceRootDir + "TestData.sln";
var projectPath_TestData = sourceRootDir + "TestData/TestData.csproj";
var nugetPackageName_TestData = "SoterDevelopment.TestData";

GitVersion version = null;

var resolveVersionTask = Task("Resolve-Version")
  .Does(() =>
  {
		version = GitVersion(new GitVersionSettings {
			UpdateAssemblyInfo = true
		});    
    Information($"GitVersion.FullSemVer: {version.FullSemVer}");
  });
  
var cleanBuildOutputDirTask = Task("Clean-BuildOutputDir")
  .Does(() =>
  {
    CleanDirectory(buildOutputDir);
  });

var buildSolutionTask = Task("Build-Solution")
  .IsDependentOn(resolveVersionTask)
  .IsDependentOn(cleanBuildOutputDirTask)
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
    foreach (var project in testProjectFiles)
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
  
var buildTargetValidateParamsTask = Task("Build-ValidateParams")
  .Does(() =>
  {
  });

var buildTask = Task("Build")
  .IsDependentOn(buildTargetValidateParamsTask)
  .IsDependentOn(buildSolutionTask)
  .IsDependentOn(runUnitTestsTask)
  .Does(() =>
  {
  });
  
var publishNuGetPackageTask = Task("Publish-NuGetPackage")
  .Does(() =>
  {
    string nugetPackageFileName = $"{nugetPackageName_TestData}.{version.SemVer}.nupkg";
    string nugetPackageFilePath = buildOutputDir + nugetPackageFileName;
    Information($"Expected package file path: {nugetPackageFilePath}");
    if (!FileExists(nugetPackageFilePath))
      throw new Exception("Could not find find package: {nugetPackageFilePath}.");
    
    Information($"Publishing package '{nugetPackageFilePath}' to repository '{nugetPushSource}'.");
    NuGetPush(nugetPackageFilePath,
      new NuGetPushSettings
      {
        Source = nugetPushSource,
        ApiKey = nugetApiKey
      });
  });

var publishTargetValidateParamsTask = Task("Publish-ValidateParams")
  .Does(() =>
  {
    if (string.IsNullOrWhiteSpace(nugetApiKey))
      throw new Exception("-NuGetApiKey param is required to publish packages."); 
  });
  
var publishTask = Task("Publish")
  .IsDependentOn(publishTargetValidateParamsTask)
  .IsDependentOn(buildTask)
  .IsDependentOn(publishNuGetPackageTask)
  .Does(() =>
  {
  });

RunTarget(target);