using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
	public class ChristmasWorkBench : ModTile
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
			TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			AddMapEntry(new Color(143, 97, 86), name);
            DustType = DustID.WoodFurniture;
			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			AdjTiles = new int[] { TileID.WorkBenches };
		}

		public override void NumDust(int x, int y, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }
    }
}