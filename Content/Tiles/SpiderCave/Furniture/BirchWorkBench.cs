using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpiderCave.Furniture
{
	public class BirchWorkBench : ModTile
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
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(155, 153, 153), Lang.GetItemName(ItemID.WorkBench));
            DustType = DustID.Web;
			AdjTiles = new int[] { TileID.WorkBenches };
		}

		public override void NumDust(int x, int y, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }
	}
}