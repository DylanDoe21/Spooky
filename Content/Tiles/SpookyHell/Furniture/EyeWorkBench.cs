using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeWorkBench : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 18 };
			TileObjectData.newTile.DrawYOffset = -2;
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Work Bench");
			AddMapEntry(new Color(114, 13, 39), name);
            DustType = DustID.Blood;
			AdjTiles = new int[] { TileID.WorkBenches };
		}

		public override void NumDust(int x, int y, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }

		public override void KillMultiTile(int x, int y, int frameX, int frameY)
		{
			Item.NewItem(new EntitySource_TileBreak(x, y), x * 16, y * 16, 32, 16, ModContent.ItemType<EyeWorkBenchItem>());
		}
	}
}