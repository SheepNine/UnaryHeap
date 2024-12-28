if ((Get-ChildItem -Recurse -Path . -Include '*.csproj' | Select-String -Pattern "<TargetFramework>(.*)</TargetFramework>" | Select-Object Line -Unique).Length -gt 1) {
	echo ".csproj files in this repository do not share a common TargetFramework version"
	exit 1
} else {
	exit 0
}
