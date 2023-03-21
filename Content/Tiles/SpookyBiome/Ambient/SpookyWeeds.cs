/*using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyWeedsOrange : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(175, 102, 36));
            DustType = ModContent.DustType<SpookyGrassDust>();
            HitSound = SoundID.Grass;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            if (Main.rand.Next(20) == 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<SpookySeedsOrange>());
            }
        }
    }

    public class SpookyWeedsGreen : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            TileID.Sets.SwaysInWindBasic[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(62, 95, 38));
            DustType = ModContent.DustType<SpookyGrassDustGreen>();
            HitSound = SoundID.Grass;
        }

        public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
        {
            if (i % 2 == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            if (Main.rand.Next(20) == 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<SpookySeedsGreen>());
            }
        }
    }
}
*/