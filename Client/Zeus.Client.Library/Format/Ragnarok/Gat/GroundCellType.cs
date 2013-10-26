namespace Zeus.Client.Library.Format.Ragnarok.Gat {

    public enum GroundCellType {
        /// <summary>
        ///     walkable, snipable, no water
        /// </summary>
        Walkable = 0,

        /// <summary>
        ///     Not walkable, not snipable, no water
        /// </summary>
        NoWalkable = 1,

        /// <summary>
        ///     Not walkable, not snipable, water
        /// </summary>
        NoWalkableNoSnipableWater = 2,

        /// <summary>
        ///     walkable, snipable, water
        /// </summary>
        WalkableSnipableWater = 3,

        /// <summary>
        ///     Not walkable, snipable, water
        /// </summary>
        NoWalkableSnipableWater = 4,

        /// <summary>
        ///     Not walkable, snipable, no water
        /// </summary>
        Snipable = 5,

        /// <summary>
        ///     Walkable, snipable, no water
        /// </summary>
        Walkable2 = 6,

        /// <summary>
        ///     Unknown type
        /// </summary>
        Unknown = 10
    };

}