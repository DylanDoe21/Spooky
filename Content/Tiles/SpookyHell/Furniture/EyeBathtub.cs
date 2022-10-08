using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeBathtub : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Bathtub");
			AddMapEntry(new Color(114, 13, 39), name);
			DustType = DustID.Blood;
			AdjTiles = new int[] { TileID.Bathtubs };
		}

		public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

		public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<EyeBathtubItem>());
        }
	}
}