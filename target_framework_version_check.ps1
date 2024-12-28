if ((Get-ChildItem -Recurse -Path . -Include '*.csproj' | Select-String -Pattern "<TargetFramework>(.*)</TargetFramework>" | Select-Object Line -Unique).Length -gt 1) {
	echo ".csproj files in this repository do not share a common TargetFramework version"
	exit 1
}

if ((Get-ChildItem -Recurse -Path . -Include '*.csproj' | Select-String -Pattern "<AssemblyVersion>(.*)</AssemblyVersion>" | Select-Object Line -Unique).Length -gt 1) {
	echo ".csproj files in this repository do not share a common AssemblyVersion version"
	exit 1
}

if ((Get-ChildItem -Recurse -Path . -Include '*.csproj' | Select-String -Pattern "<FileVersion>(.*)</FileVersion>" | Select-Object Line -Unique).Length -gt 1) {
	echo ".csproj files in this repository do not share a common FileVersion version"
	exit 1
}

exit 0