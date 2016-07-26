using System;

namespace CoordSaverV
{
    [Serializable]
    public struct SpawnPoint
    {
        public Rage.Vector3 Position;
        public float Heading;

        public SpawnPoint(float fHeading, Rage.Vector3 v3Position)
        {
            Heading = fHeading;
            Position = v3Position;
        }
    }
}