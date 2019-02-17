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

string version = "0.0.0.0";
string assemblyVersion = "0.0.0.0";
string fileVersion = "0.0.0.0";

var resolveVersionTask = Task("Resolve-Version")
  .Does(() =>
  {
		var gitVersion = GitVersion(new GitVersionSettings {
			UpdateAssemblyInfo = false
		});
    
    version = gitVersion.FullSemVer;
    Information($"version (GitVersion.FullSemVer): {version}");
    
    assemblyVersion = gitVersion.AssemblySemVer;
    Information($"assemblyVersion (GitVersion.AssemblySemVer): {assemblyVersion}");
    
    fileVersion = "1.0.0.0";
    Information($"fileVersion (fixed): {fileVersion}");
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
          .Append($"-p:Version={version}")
          .Append($"-p:AssemblyVersion={assemblyVersion}")
          .Append($"-p:FileVersion={fileVersion}")
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
  
var buildNuGetPackageTask = Task("Build-NuGetPackage")
  .IsDependentOn(resolveVersionTask)
  .IsDependentOn(buildSolutionTask)
  .IsDependentOn(runUnitTestsTask)
  .Does(() =>
  {
    NuGetPack(projectPath_TestData,
      new NuGetPackSettings
      {
        OutputDirectory = nugetOutputDir,
        Version = version,
        ArgumentCustomization = args => args
          .Append($"-Properties Version={version}")
          .Append($"-Properties AssemblyVersion={assemblyVersion}")
          .Append($"-Properties FileVersion={fileVersion}")
          //.Append($"-Properties Version={version};AssemblyVersion={assemblyVersion};FileVersion={fileVersion}")
      });
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