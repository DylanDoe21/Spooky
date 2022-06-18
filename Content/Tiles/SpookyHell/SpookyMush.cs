using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class SpookyMush : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileMerge[Type][TileID.Ash] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(88, 49, 129));
            ItemDrop = ModContent.ItemType<SpookyMushItem>();
			DustType = -1;
            HitSound = SoundID.Dig;
		}
	}
}
