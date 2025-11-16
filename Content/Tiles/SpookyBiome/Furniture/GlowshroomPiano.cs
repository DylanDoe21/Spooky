using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class GlowshroomPiano : ModTile
	{
        public override void SetStaticDefaults()
		{
            Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(122, 72, 203), Lang.GetItemName(ItemID.Piano));
            DustType = DustID.Slush;
			AdjTiles = new int[] { TileID.Pianos };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }
    }

	public class GlowshroomYellowPiano : GlowshroomPiano
	{
        public override void SetStaticDefaults()
		{
            Main.tileSolidTop[Type] = true;
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(179, 128, 50), Lang.GetItemName(ItemID.Piano));
            DustType = DustID.Slush;
			AdjTiles = new int[] { TileID.Pianos };
		}
	}
}