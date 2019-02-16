var target = Argument("target", "Build");
var configuration = Argument("Configuration", "Release");
var dotNetCoreVerbosity = (DotNetCoreVerbosity)Enum.Parse(typeof(DotNetCoreVerbosity), Argument("DotNetCoreVerbosity", "Normal"));

var outputRootDir = "./build/";
var buildOutputDir = outputRootDir + "artifacts/";
var testsOutputDir = outputRootDir + "tests/";
var solutionPath_TestData = "./src/TestData.sln";


var buildSolutionTask = Task("Build-Solution")
  .Does(() =>
  {
    DotNetCoreBuild(solutionPath_TestData,
      new DotNetCoreBuildSettings
      {
        Configuration = configuration,
        OutputDirectory = buildOutputDir
      });
  });
  
var runUnitTestsTask = Task("Run-UnitTests")
  .IsDependentOn(buildSolutionTask)
  .Does(() =>
  {
    var projectFiles = GetFiles("./src/**/*.Tests.csproj");
    foreach(var project in projectFiles)
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
  
var buildNuGetPackageTask = Task("Build-NuGetPackage")
  .IsDependentOn(buildSolutionTask)
  .IsDependentOn(runUnitTestsTask)
  .Does(() =>
  {
  });

var buildTask = Task("Build")
  .IsDependentOn(buildSolutionTask)
  .IsDependentOn(runUnitTestsTask)
  .IsDependentOn(buildNuGetPackageTask)
  .Does(() =>
  {
  });
  
var publishNuGetPackageTask = Task("Publish-NuGetPackage")
  .Does(() =>
  {
  });

Task("Publish")
  .IsDependentOn(buildTask)
  .IsDependentOn(publishNuGetPackageTask)
  .Does(() =>
  {
  });

RunTarget(target);