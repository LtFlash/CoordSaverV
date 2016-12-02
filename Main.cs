using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Rage;
using Rage.Attributes;
using Rage.ConsoleCommands.AutoCompleters;
using System.Drawing;

/* Changes:
 * 2016-12-03:
 * - Command_TranslateXmlToTxt
 * 2016-07-25:
 * - ISpawnPointWriter
 * - added SetOutputFormat
 * - finalizer
 * - LoadBlipsFromAllFiles
 * 2016-06-15:
 * - AutoTag,
 * - AutoSave,
 * - blips; create, load, change color,
 * - zone name translation,
 * - AutoCompleters
 * - ini
 * 2015-08-31 First created
 */

//TODO:
// - Command_LoadFromFileAndTranslateZones - check if value/key exists
// - add: LoadBlipsFromAllFiles (xmls)

[assembly: Plugin(
    "CoordSaverV",
    Author = "LtFlash",
    Description ="Developer's tool to manually collect spawn points.",
    PrefersSingleInstance = true)]

namespace CoordSaverV
{
    public static class EntryPoint
    {
        internal const string HOME_FOLDER = @".\Plugins\CoordSaverV\";
        private static string filePath = string.Empty;

        private static ISpawnPointWriter spawnPointWriter = new SpawnPointWriterXml();
        private static StaticFinalizer finalizer;
        private static bool canRun;
        private static ExtendedSpawnPoint espCurrent = new ExtendedSpawnPoint();
        private static string permTag = string.Empty;
        private static List<Blip> blips = new List<Blip>();
        private static Color blipColor = Color.Red;

        public static void Main()
        {
            Game.Console.Print("*****CoordSaverV 2.1 by LtFlash*****");
            Game.Console.Print("Type \"CoordSaverV\" to display help.");

            Settings.LoadFromFile();

            finalizer = new StaticFinalizer(Finalizer);
            canRun = true;

            while (canRun)
            {
                if (Game.IsKeyDownRightNow(Settings.AddSpawnModifier) && Game.IsKeyDown(Settings.AddSpawnKey))
                {
                    Command_AddSpawn();
                }
                if (Game.IsKeyDownRightNow(Settings.SetZoneModifier) && Game.IsKeyDown(Settings.SetZoneKey))
                {
                    Command_SetZone();
                }
                if(Game.IsKeyDownRightNow(Settings.SaveSpawnModifier) && Game.IsKeyDown(Settings.SaveSpawnKey))
                {
                    Command_SaveSpawn();
                }

                GameFiber.Yield();
            }
        }

        private static void Finalizer()
        {
            canRun = false;
            Command_RemoveAllBlips();
        }

        [ConsoleCommand]
        public static void Command_SetFileName([ConsoleCommandParameter(AutoCompleterType = typeof(FileNameCompleterExtensionDependened))] string fileName)
        {
            string fileN = ValidateFileName(fileName, spawnPointWriter.FileExtension);

            filePath = Path.Combine(HOME_FOLDER, fileN);
            
            Game.Console.Print("Current file path: " + filePath);
        }
        
        private static string ValidateFileName(string fileName, string extension)
        {
            string ext = $".{extension}";
            if (fileName.Trim().EndsWith(ext)) return fileName;
            else return fileName + ext;
        }

        private static string CheckFileExistence(string fileName, string extension)
        {
            string pathToFile = Path.Combine(HOME_FOLDER, ValidateFileName(fileName, extension));

            return File.Exists(pathToFile) ? pathToFile : "";
        }

        [ConsoleCommand]
        public static void Command_SetOutputFormat([ConsoleCommandParameter(AutoCompleterType = typeof(OutputFormatCompleter))] string format)
        {
            if (format == "xml")
            {
                spawnPointWriter = new SpawnPointWriterXml();
                filePath = string.Empty;
            }
            else if (format == "txt")
            {
                spawnPointWriter = new SpawnPointWriterTxt();
                filePath = string.Empty;
            }
            else Game.Console.Print("Wrong format!");
        }

        [ConsoleCommand]
        public static void Command_TranslateXmlToTxt([ConsoleCommandParameter(AutoCompleterType = typeof(FileNameCompleter))] string sourceFile, string destFile)
        {
            var path = Path.Combine(HOME_FOLDER, sourceFile);
            if (!File.Exists(path))
            {
                Game.Console.Print("Specified source file does not exist!");
                return;
            }
            var sp = Serialization.LoadFromXML<ExtendedSpawnPoint>(path);
            var s = new SpawnPointWriterTxt();
            var pathDest = Path.Combine(HOME_FOLDER, destFile);
            s.FileName = ValidateFileName(pathDest, "txt");
            for (int i = 0; i < sp.Count; i++)
            {
                s.Save(sp[i]);
            }
            Game.Console.Print("Done!");
        }

        [ConsoleCommand]
        public static void Command_AddTag(string tag)
        {
            espCurrent.Tags.Add(tag);

            DisplayInfo($"Tag [{tag}] added!");
        }
        
        [ConsoleCommand]
        public static void Command_AddSpawn() //TODO: AutoCompleter; car/ped pos etc.
        {
            espCurrent.Spawn.Add(new SpawnPoint(Game.LocalPlayer.Character.Heading, Game.LocalPlayer.Character.Position));

            DisplayInfo("SpawnPoint added, current count: " + espCurrent.Spawn.Count);

            if (Settings.CreateBlips)
            {
                Blip b = new Blip(Game.LocalPlayer.Character.Position);
                b.Color = blipColor;
                blips.Add(b);
            }
            if (Settings.AutoSave) //last because it'll create a new ESP
            {
                Command_SetZone();
                Command_SaveSpawn();
            }
        }

        [ConsoleCommand]
        public static void Command_SetAutoTag(string autoTag)
        {
            permTag = autoTag;

            DisplayInfo("AutoTag active! Current tag: " + permTag);
        }

        [ConsoleCommand]
        public static void Command_RemoveAutoTag()
        {
            permTag = string.Empty;
            DisplayInfo("AutoTag removed!");
        }

        [ConsoleCommand]
        public static void Command_SetBlipsCreation(bool createBlips)
        {
            Settings.CreateBlips = createBlips;
            DisplayInfo("Blips creation: " + (createBlips ? "on" : "off"));
        }

        [ConsoleCommand]
        public static void Command_SetZoneNameTranslation(bool translate)
        {
            Settings.TranslateZoneNames = translate;
            DisplayInfo("Zone name translation: " + (translate ? "on" : "off"));
        }

        [ConsoleCommand]
        public static void Command_SetBlipsColor(string color)
        {
            blipColor = System.Drawing.Color.FromName(color);
            DisplayInfo("Blips color set to: " + blipColor.ToString());
        }

        [ConsoleCommand]
        public static void Command_LoadBlipsFromFile([ConsoleCommandParameter(AutoCompleterType = typeof(FileNameCompleter))] string fileName)
        {
            //can only load from xmls
            string pathToFile = Path.Combine(HOME_FOLDER, ValidateFileName(fileName, "xml"));
            if (!File.Exists(pathToFile))
            {
                DisplayInfo("File does not exist!");
                return;
            }

            List<ExtendedSpawnPoint> esps = Serialization.LoadFromXML<ExtendedSpawnPoint>(pathToFile);

            Blip b;
            int count = 0;
            for (int i = 0; i < esps.Count; i++)
            {
                for (int j = 0; j < esps[i].Spawn.Count; j++)
                {
                    if (esps[i].Spawn[j].Position == Vector3.Zero) continue;

                    b = new Blip(esps[i].Spawn[j].Position);
                    b.Color = blipColor;
                    blips.Add(b);
                    count++;
                }
            }

            if(count > 0) DisplayInfo("Blips added to your map, count: " + count);
        }

        [ConsoleCommand(Description = "from xml files only")]
        public static void Command_LoadBlipsFromAllFiles()
        {
            string[] files = Directory.GetFiles(HOME_FOLDER, "*.xml", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < files.Length; i++)
            {
                Command_LoadBlipsFromFile(files[i]);
            }
        }

        [ConsoleCommand]
        public static void Command_TranslateZonesInFile(
            [ConsoleCommandParameter(AutoCompleterType = typeof(FileNameCompleter))] string fileName,
            [ConsoleCommandParameter(Description ="true: abbrev->full name; false: full name->abbrev")] bool fromAbbrevToFull = true)
        {
            string pathToFile = CheckFileExistence(fileName, "xml");
            if (pathToFile == string.Empty)
            {
                DisplayInfo("File does not exist!");
                return;
            }

            List<ExtendedSpawnPoint> esps = Serialization.LoadFromXML<ExtendedSpawnPoint>(pathToFile);

            for (int i = 0; i < esps.Count; i++)
            {
                if (fromAbbrevToFull) esps[i].Zone = Zones.ZoneNames[esps[i].Zone];
                else esps[i].Zone = Zones.ZoneNames.FirstOrDefault(e => e.Value == esps[i].Zone).Key;
            }

            string pathToFile_Translated = Path.Combine(
                Path.GetDirectoryName(pathToFile),
                Path.GetFileNameWithoutExtension(pathToFile) + "_T.xml");

            Serialization.SaveToXML(esps, pathToFile_Translated);

            DisplayInfo("Zone names translation has finished!");
        }

        [ConsoleCommand]
        public static void Command_RemoveAllBlips()
        {
            for (int i = 0; i < blips.Count; i++)
            {
                if (blips[i]) blips[i].Delete();
            }

            blips.Clear();
        }

        [ConsoleCommand]
        public static void Command_SetZone()
        {
            string zoneName = string.Empty;
            Vector3 Position = Game.LocalPlayer.Character.Position;
            unsafe
            {
                IntPtr ptr = Rage.Native.NativeFunction.CallByName<IntPtr>(
                    "GET_NAME_OF_ZONE",
                    Position.X, Position.Y, Position.Z);

                zoneName = Marshal.PtrToStringAnsi(ptr);
            }

            if (Settings.TranslateZoneNames) zoneName = Zones.ZoneNames[zoneName];

            espCurrent.Street = World.GetStreetName(Position);
            espCurrent.Zone = zoneName;

            DisplayInfo($"Zone: {espCurrent.Zone}, Street: {espCurrent.Street}");
        }

        [ConsoleCommand]
        public static void Command_SaveSpawn()
        {
            if (filePath == string.Empty)
            {
                Game.DisplayNotification("~r~File name has not been set!");
                return;
            }

            if (espCurrent.Spawn.Count == 0)
            {
                Game.DisplayNotification("~r~SpawnPoint list is empty!");
                return;
            }

            if (espCurrent.Zone == string.Empty)
            {
                Game.DisplayNotification("~r~Zone has not been set!");
                return;
            }

            if (permTag != string.Empty) Command_AddTag(permTag);

            spawnPointWriter.FileName = filePath;
            spawnPointWriter.Save(espCurrent);

            DisplayInfo("~g~ExtendedSpawnPoint point saved!");

            espCurrent = new ExtendedSpawnPoint();
        }

        [ConsoleCommand]
        public static void Command_SetAutoSave([ConsoleCommandParameter(Description ="true: save to file after adding the 1st SpawnPoint")] bool autosaveActive)
        {
            Settings.AutoSave = autosaveActive;
            DisplayInfo("AutoSave status: " + (autosaveActive ? "on" : "off"));
        }

        private static void DisplayInfo(string msg)
        {
            Game.Console.Print(msg);
            Game.DisplayNotification(msg);
        }

        [ConsoleCommand]
        public static void Command_CoordSaverV()
        {
            Game.Console.Print("*****CoordSaverV 2.1 by LtFlash*****");
            Game.Console.Print("* Basic commands:");
            Game.Console.Print("1. SetFileName to define where to save your spawns");
            Game.Console.Print(string.Format("2. AddSpawn ({0} + {1})", Settings.AddSpawnModifier, Settings.AddSpawnKey));
            Game.Console.Print("3. AddTag to add tags");
            Game.Console.Print(string.Format("4. SetZone ({0} + {1})", Settings.SetZoneModifier, Settings.SetZoneKey));
            Game.Console.Print(string.Format("5. SaveSpawn ({0} + {1})", Settings.SaveSpawnModifier, Settings.SaveSpawnKey));

            Game.Console.Print("* Additional commands:");
            Game.Console.Print(" - SetBlipsCreation; - SetBlipsColor; - LoadBlipsFromFile; - RemoveAllBlips");
            Game.Console.Print(" - SetAutoTag; - RemoveAutoTag; - SetZoneNameTranslation; - SetAutoSave");
            Game.Console.Print(" - TranslateZonesInFile; - SetOutputFormat; - TranslateXmlToTxt;");
        }

        [Serializable]
        [ConsoleCommandParameterAutoCompleter(typeof(string))]
        private class FileNameCompleter : ConsoleCommandParameterAutoCompleter
        {
            public FileNameCompleter(Type type)
                : base(type)
            {
            }

            public override void UpdateOptions()
            {
                string[] files = Directory.GetFiles(HOME_FOLDER, "*.xml", SearchOption.AllDirectories);
                Options.Clear();
                for (int i = 0; i < files.Length; i++)
                {
                    Options.Add(new Rage.ConsoleCommands.AutoCompleteOption(Path.GetFileName(files[i]), null, null));
                }
            }
        }

        [Serializable]
        [ConsoleCommandParameterAutoCompleter(typeof(string))]
        private class FileNameCompleterExtensionDependened : ConsoleCommandParameterAutoCompleter
        {
            public FileNameCompleterExtensionDependened(Type type)
                : base(type)
            {
            }

            public override void UpdateOptions()
            {
                string[] files = Directory.GetFiles(HOME_FOLDER, $"*.{spawnPointWriter.FileExtension}", SearchOption.AllDirectories);
                Options.Clear();
                for (int i = 0; i < files.Length; i++)
                {
                    Options.Add(new Rage.ConsoleCommands.AutoCompleteOption(Path.GetFileName(files[i]), null, null));
                }
            }
        }

        [Serializable]
        [ConsoleCommandParameterAutoCompleter(typeof(string))]
        private class OutputFormatCompleter : ConsoleCommandParameterAutoCompleter
        {
            public OutputFormatCompleter(Type type) : base(type)
            {
            }

            public override void UpdateOptions()
            {
                Options.Add(new Rage.ConsoleCommands.AutoCompleteOption("xml", null, null));
                Options.Add(new Rage.ConsoleCommands.AutoCompleteOption("txt", null, null));

            }
        }
    }
}
