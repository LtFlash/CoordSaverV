using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace CoordSaverV
{
    internal class SpawnPointWriterTxt : ISpawnPointWriter
    {
        public string FileName { get; set; }

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

            SaveToFile(linesToSave);
        }

        private void SaveToFile(List<string> lines)
        {
            using (var file = new StreamWriter(FileName, true))
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    file.WriteLine(lines[i]);
                }
                file.WriteLine();

                file.Flush();
            }
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
                string lineSpawn = $"new SpawnPoint({sp.Heading.ToString(CultureInfo.InvariantCulture)}f, new Vector3({sp.Position.X.ToString(CultureInfo.InvariantCulture)}f, {sp.Position.Y.ToString(CultureInfo.InvariantCulture)}f, {sp.Position.Z.ToString(CultureInfo.InvariantCulture)}f));";

                lines.Add(lineSpawn);
            }

            return lines;
        }
    }
}
