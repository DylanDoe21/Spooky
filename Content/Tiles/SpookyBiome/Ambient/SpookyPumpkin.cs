using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.SpookyBiome.Misc;

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
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(214, 106, 49));
            DustType = 288;
            HitSound = SoundID.Dig;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
            //rot gourd material
            if (Main.rand.NextBool(2) && Flags.downedRotGourd)
            {
			    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<RottenChunk>());
            }

            //rot gourd summon
            if (Main.rand.NextBool(6))
            {
			    Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 16, ModContent.ItemType<RottenSeed>());
            }

            //spawn gores
            if (Main.netMode != NetmodeID.Server)
			{
                Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, new Vector2(0, 0), ModContent.Find<ModGore>("Spooky/PumpkinTileGore1").Type);
                Gore.NewGore(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16, new Vector2(0, 0), ModContent.Find<ModGore>("Spooky/PumpkinTileGore2").Type);
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