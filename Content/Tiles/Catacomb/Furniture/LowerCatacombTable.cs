using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
    public class LowerCatacombTable : ModTile
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
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(86, 46, 29), Language.GetText("MapObject.Table"));
            DustType = DustID.Grass;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			AdjTiles = new int[] { TileID.Tables };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
		{
			num = fail ? 1 : 3;
		}
    }
}