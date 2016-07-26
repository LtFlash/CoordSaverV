using System.Windows.Forms;
using System.IO;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Rage;

namespace CoordSaverV
{
    internal static class Settings
    {
        private static InitializationFile ini = new InitializationFile(@".\Plugins\CoordSaverV.ini");

        internal static bool CreateBlips
        {
            get { return _createBlips; }
            set { _createBlips = value; ini.Write("Configuration", "CreateBlips", _createBlips); }
        }
        private static bool _createBlips = true;

        internal static bool TranslateZoneNames
        {
            get { return _translateZoneNames; }
            set { _translateZoneNames = value; ini.Write("Configuration", "TranslateZoneNames", _translateZoneNames); }
        }
        private static bool _translateZoneNames = true;

        internal static bool AutoSave
        {
            get { return _autoSave; }
            set { _autoSave = value; ini.Write("Configuration", "AutoSave", _autoSave); }
        }
        private static bool _autoSave = false;

        internal static Keys AddSpawnKey
        {
            get { return _addSpawnKey; }
            set { _addSpawnKey = value; ini.Write("Keys", "AddSpawnKey", _addSpawnKey); }
        }
        private static Keys _addSpawnKey = Keys.F10;

        internal static Keys AddSpawnModifier
        {
            get { return _addSpawnModifier; }
            set { _addSpawnModifier = value; ini.Write("Keys", "AddSpawnModifier", _addSpawnModifier); }
        }
        private static Keys _addSpawnModifier = Keys.LShiftKey;

        internal static Keys SetZoneKey
        {
            get { return _setZoneKey; }
            set { _setZoneKey = value; ini.Write("Keys", "SetZoneKey", _setZoneKey); }
        }
        private static Keys _setZoneKey = Keys.F11;

        internal static Keys SetZoneModifier
        {
            get { return _setZoneModifier; }
            set { _setZoneModifier = value; ini.Write("Keys", "SetZoneModifier", _setZoneModifier); }
        }
        private static Keys _setZoneModifier = Keys.LShiftKey;

        internal static Keys SaveSpawnKey
        {
            get { return _saveSpawnKey; }
            set { _saveSpawnKey = value; ini.Write("Keys", "SaveSpawnKey", _saveSpawnKey); }
        }
        private static Keys _saveSpawnKey = Keys.F12;

        internal static Keys SaveSpawnModifier
        {
            get { return _saveSpawnModifier; }
            set { _saveSpawnModifier = value; ini.Write("Keys", "SaveSpawnModifier", _saveSpawnModifier); }
        }
        private static Keys _saveSpawnModifier = Keys.LShiftKey;

        internal static void LoadFromFile()
        {
            //Game.Console.Print("Ini file existance check: " + ini.Exists().ToString());

            ValidateIniFile();

            _createBlips = ini.ReadBoolean("Configuration", "CreateBlips", _createBlips);
            _translateZoneNames = ini.ReadBoolean("Configuration", "TranslateZoneNames", _translateZoneNames);
            _autoSave = ini.ReadBoolean("Configuration", "AutoSave", _autoSave);

            _addSpawnKey = ini.ReadEnum("Keys", "AddSpawnKey", _addSpawnKey);
            _addSpawnModifier = ini.ReadEnum("Keys", "AddSpawnModifier", _addSpawnModifier);

            _setZoneKey = ini.ReadEnum("Keys", "SetZoneKey", _setZoneKey);
            _setZoneModifier = ini.ReadEnum("Keys", "SetZoneModifier", _setZoneModifier);

            _saveSpawnKey = ini.ReadEnum("Keys", "SaveSpawnKey", _saveSpawnKey);
            _saveSpawnModifier = ini.ReadEnum("Keys", "SaveSpawnModifier", _saveSpawnModifier);
        }

        private static Keys ParseStringToKey(string key, Keys def)
        {
            Keys result;
            bool success = Enum.TryParse(key, out result);
            if (success) return result;
            else return def;
        }

        private static void ValidateIniFile()
        {
            //check existance, if not -> create new
            if (ini.Exists()) return;

            //Game.Console.Print("Creating ini file.");

            string[] iniLines = new string[]
            {
                "[Configuration]",
                "CreateBlips=" + _createBlips,
                "TranslateZoneNames=" + _translateZoneNames,
                "AutoSave=" + _autoSave,
                "",
                "[Keys]",
                "AddSpawnKey=" + _addSpawnKey,
                "AddSpawnModifier=" +_addSpawnModifier,
                "",
                "SetZoneKey=" + _setZoneKey,
                "SetZoneModifier=" +_setZoneModifier,
                "",
                "SaveSpawnKey=" + _saveSpawnKey,
                "SaveSpawnModifier=" + _saveSpawnModifier,
            };

            using (StreamWriter fs = File.AppendText(ini.FileName))
            {
                for (int i = 0; i < iniLines.Length; i++)
                {
                    fs.WriteLine(iniLines[i]);
                }
                fs.Close();
            }

            Game.Console.Print("Ini file has been created!");
        }
    }
}
