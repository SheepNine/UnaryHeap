# Development Guide

## Setting up git hooks

Run the following command after checking out to ensure that you get the lint-rolling pre-commit hook. Failing to do so may mean that your
builds fail in Github instead.

`git config core.hooksPath .githooks`

## Manual delinting

To see editor guidelines in your Visual Studio IDE, install the ['Editor Guidelines' extension](https://marketplace.visualstudio.com/items?itemName=PaulHarrington.EditorGuidelinesPreview).

If you want to run the lint roller without having to use the pre-commit hook, run the following command:

`dotnet build -t:Unlint`

## Making Releases

From the project root directory, invoke `dotnet build -c Release`. This will do a clean/compile/lint/package cycle.

## Release Checklist

* Review code coverage metrics for newly-added code
* Update project AssemblyVersion/FileVersion properties to the target release number
* Update project TargetFrameworks property to latest LTS release according to https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
  * The setup-dotnet action
  * build.proj target framework default
  * TargetFramework value in _all_ .csproj files (remember there are multiple solutions!)
* Upgrade project Nuget packages to latest stable
* Read over and revise documentation in HTML/MD files
* Commit everything and run it through the Github CD build
* Download resulting artifacts and smoke test them
  * Make sure the apps actually start; no files are missing
  * Compare documentation to actual behaviour; check for correctness
* Merge develop into master
* Create a tag for that commit
* Create a release in Github for that tag, adding release notes and copying the build artifacts into the release assets
* Update project AssemblyVersion/FileVersion properties again on the develop branch to the next target release number