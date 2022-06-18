using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyRock1 : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(12, 49, 10));
            DustType = DustID.Stone;
			HitSound = SoundID.Dig;
		}
	}

	public class SpookyRock2 : SpookyRock1
    {
	}

	public class SpookyRock3 : SpookyRock1
    {
	}
}