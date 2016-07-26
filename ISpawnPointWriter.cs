namespace CoordSaverV
{
    interface ISpawnPointWriter
    {
        string FileExtension { get; }
        string FileName { get; set; } 
        void Save(ExtendedSpawnPoint esp);
    }
}
