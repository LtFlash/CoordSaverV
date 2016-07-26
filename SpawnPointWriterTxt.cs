using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace CoordSaverV
{
    internal class SpawnPointWriterTxt : ISpawnPointWriter
    {
        public string FileName { get; set; }
        private StreamWriter _file;

        public string FileExtension
        {
            get
            {
                return "txt";
            }
        }

        public void Save(ExtendedSpawnPoint esp)
        {
            List<string> linesToSave = new List<string>();

            linesToSave = CreateLines(esp);

            for (int i = 0; i < linesToSave.Count; i++)
            {
                _file.WriteLine(linesToSave[i]);
            }
            _file.WriteLine();

            _file.Close();
        }

        private List<string> CreateLines(ExtendedSpawnPoint esp)
        {
            List<string> lines = new List<string>();
            string lineTags = "//" + string.Join(", ", esp.Tags);
            string lineArea = "//" + string.Join(", ", esp.Zone, esp.Street);

            lines.Add(lineTags);
            lines.Add(lineArea);

            foreach (var sp in esp.Spawn)
            {
                string lineSpawn = $"new SpawnPoint({sp.Heading.ToString(CultureInfo.InvariantCulture)}f, {sp.Position.X.ToString(CultureInfo.InvariantCulture)}f, {sp.Position.Y.ToString(CultureInfo.InvariantCulture)}f, {sp.Position.Z.ToString(CultureInfo.InvariantCulture)}f);";

                lines.Add(lineSpawn);
            }

            return lines;
        }
    }
}
