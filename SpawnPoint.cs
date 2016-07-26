using System;

namespace CoordSaverV
{
    [Serializable]
    public struct SpawnPoint
    {
        public Rage.Vector3 Position;
        public float Heading;

        public SpawnPoint(float heading, Rage.Vector3 position)
        {
            Heading = heading;
            Position = position;
        }
    }
}