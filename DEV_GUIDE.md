# Development Guide

## Making Releases

From the project root directory, invoke `dotnet build -c Release`. This will do a clean/compile/lint/package cycle.

## Release Checklist

* Update project AssemblyVersion/FileVersion properties to the target release number
* Update project TargetFrameworks property to latest LTS release according to https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core
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