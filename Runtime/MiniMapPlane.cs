namespace Arikan
{
    /// <summary>
    /// World plane represented by the mini map.
    /// </summary>
    public enum MiniMapPlane
    {
        /// <summary>Top-down 3D worlds: world X is map right, world Z is map up.</summary>
        XZ = 0,

        /// <summary>2D worlds: world X is map right, world Y is map up.</summary>
        XY = 1
    }
}
