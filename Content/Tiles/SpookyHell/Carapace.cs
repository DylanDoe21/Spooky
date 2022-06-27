using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class Carapace : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileMerge[Type][ModContent.TileType<SpookyMush>()] = true;
			Main.tileMerge[Type][ModContent.TileType<EyeBlock>()] = true;
            Main.tileMerge[Type][TileID.Ash] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(189, 219, 191));
            ItemDrop = ModContent.ItemType<CarapaceItem>();
			DustType = -1;
            HitSound = SoundID.Dig;
		}
	}
}
