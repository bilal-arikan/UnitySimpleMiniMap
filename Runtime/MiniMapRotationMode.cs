namespace Arikan
{
    /// <summary>
    /// How the map rotates while a centered target is followed.
    /// </summary>
    public enum MiniMapRotationMode
    {
        /// <summary>The map keeps world up at the top and the centered icon rotates.</summary>
        NorthUp = 0,

        /// <summary>The map rotates so the centered target always faces up.</summary>
        RotateWithTarget = 1
    }
}
