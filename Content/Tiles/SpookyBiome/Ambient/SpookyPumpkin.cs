using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyPumpkin1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);   
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(161, 94, 27));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), new Vector2(0, 0), ModContent.Find<ModGore>("Spooky/PumpkinTileGore1").Type);
            Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i * 16, j * 16), new Vector2(0, 0), ModContent.Find<ModGore>("Spooky/PumpkinTileGore2").Type);

            if (Main.rand.NextBool(5))
            {
			    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<RottenSeed>());
            }
		}
    }

    public class SpookyPumpkin2 : SpookyPumpkin1
    {
    }

    public class SpookyPumpkin3 : SpookyPumpkin1
    {
    }
}