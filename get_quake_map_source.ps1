$ArchiveFilename = $PSScriptRoot + "/artifacts/quake_map_source.zip"
$ExpandedDirectory = $PSScriptRoot + "/artifacts/quake_map_source"


# Download the archive from Romero's site
if ( -not ( Test-Path $ArchiveFilename ) )
{
	Invoke-WebRequest https://rome.ro/s/quake_map_source.zip -OutFile $ArchiveFilename
}

# Extract the archive
if ( -not ( Test-Path $ExpandedDirectory ) )
{
	Expand-Archive -Path $ArchiveFilename -DestinationPath $ExpandedDirectory
	rm  "$ExpandedDirectory/b_*"
	rm  "$ExpandedDirectory/dm7.map"
	rm  "$ExpandedDirectory/dm8.map"
	rm  "$ExpandedDirectory/e2m10.*"
	rm  "$ExpandedDirectory/release_readme.txt"
	git apply quake_map_source.patch
}
