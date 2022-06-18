using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell
{
	public class EyeBlock : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileMerge[Type][TileID.Ash] = true;
			Main.tileSolid[Type] = true;
			Main.tileBlockLight[Type] = true;
            AddMapEntry(new Color(138, 31, 40));
            ItemDrop = ModContent.ItemType<EyeBlockItem>();
			DustType = DustID.Blood;
            HitSound = SoundID.Dig;
		}
	}
}
