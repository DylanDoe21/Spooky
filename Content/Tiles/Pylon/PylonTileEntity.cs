using Terraria.ModLoader.Default;

namespace Spooky.Content.Tiles.Pylon
{
	/// <summary>
	/// This is an empty child class that acts exactly like the default implementation of the abstract <seealso cref="TEModdedPylon"/>
	/// class, which itself acts nearly identical to vanilla pylon TEs. This inheritance only exists so that modded pylon entities
	/// will properly have their "Mod" property set, for I/O purposes. Has the sealed modifier since this TE acts identical to its parent.
	/// </summary>
	public sealed class PylonTileEntity : TEModdedPylon 
    {
    }
}