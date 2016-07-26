CoordSaverV 2.1 by LtFlash

New features in 2.1:
- SetOutputFormat command
- LoadBlipsFromAllFiles

Basic commands:
- SetFileName
	defines where to save your spawns.
- AddSpawn
	adds your current position as a new spawn point
	to the current ExtendedSpawnPoint.
- AddTag
	adds tag to the current ESP.
- SetZone
	set street and zone name of current position to
	the ESP.
- SaveSpawn
	saves your ESP to file.

Additional commands:
- SetOutputFormat
	sets the type of output format of data; there are 
	two available options: xml serialization, txt file
	containing SpawnPoint constructors filled with coords
	of saved spawns. Set output file name after using
	this command.
- SetAutoSave
	will save the ESP after adding the first spawn point.
- SetBlipsCreation
	defines if a blip is created for every spawn point.
- SetBlipsColor
	set color for blips.
- LoadBlipsFromFile
	loads ESPs from file and creates blips for all of them.
- LoadBlipsFromAllFiles
	load blips from all *.xml files inside the home folder.
- RemoveAllBlips
	removes all blips created by the plugin.
- SetAutoTag
	adds a predefined tag to every new ESP.
- RemoveAutoTag
	a predefined tag will no longer be added to every new ESP.
- SetZoneNameTranslation
	if set to true zone name will be saved as a full name
	("Rockford Hills") instead of an abbreviation ("ROCKF").
- TranslateZonesInFile
	loads all ESPs from file, translates names (two-way) and
	saves the result to another file.

