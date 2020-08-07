using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LXProtocols.Citp.Packets.Msex
{
    public enum MsexElementType
    {
        Media = 1,
        Effects,
        Cues,
        Crossfades,
        Masks,
        BlendPresets,
        EffectPresets,
        ImagePresets
    }

    [Flags]
    public enum MsexUpdateFlags
    {
        /// <summary>
        /// Nothing has been updated.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Existing elements have been updated
        /// </summary>
        Element = 0x01,

        /// <summary>
        /// Elements have been added or removed
        /// </summary>
        AddOrRemoveElement = 0x02,

        /// <summary>
        /// Sub libraries have been updated
        /// </summary>
        SubLibrary = 0x04,

        /// <summary>
        /// Sub libraries have been added or removed
        /// </summary>
        AddOrRemoveSubLibrary = 0x08,

        /// <summary>
        /// All elements have been affected
        /// </summary>
        AllElements = 0x10,

        /// <summary>
        /// All sub libraries have been affected
        /// </summary>
        AllSubLibraries = 0x20
    }
}
