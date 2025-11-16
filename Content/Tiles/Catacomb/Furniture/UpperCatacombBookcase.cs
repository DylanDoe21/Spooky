using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class UpperCatacombBookcase : ModTile
    {
		public override void SetStaticDefaults()
		{
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(65, 79, 88), Lang.GetItemName(ItemID.Bookcase));
            DustType = DustID.Stone;
            AdjTiles = new int[] { TileID.Bookcases };
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}