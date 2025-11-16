using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Minibiomes.Christmas.Furniture
{
    public class ChristmasSink : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16 };
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(143, 97, 86), Language.GetText("MapObject.Sink"));
            DustType = DustID.WoodFurniture;
			AdjTiles = new int[] { TileID.Sinks };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
		}
    }
}