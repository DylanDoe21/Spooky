using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class LowerCatacombPiano : ModTile
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
			AddMapEntry(new Color(86, 46, 29), Lang.GetItemName(ItemID.Piano));
            DustType = DustID.t_Lihzahrd;
			AdjTiles = new int[] { TileID.Pianos };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) 
        {
            num = fail ? 1 : 3;
        }
    }
}