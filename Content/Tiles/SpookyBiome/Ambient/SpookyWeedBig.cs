using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyWeedBig1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(175, 102, 36));
            DustType = ModContent.DustType<SpookyGrassDust>();
            HitSound = SoundID.Grass;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            if (Main.rand.Next(20) == 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<SpookySeedsOrange>());
            }
        }
    }

    public class SpookyWeedBig2 : SpookyWeedBig1
    {
    }

    public class SpookyWeedBig3 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.IgnoredByGrowingSaplings[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(62, 95, 38));
            DustType = ModContent.DustType<SpookyGrassDustGreen>();
            HitSound = SoundID.Grass;
        }

        public override void KillMultiTile(int i, int j, int frameX, int frameY) 
        {
            if (Main.rand.Next(20) == 0)
            {
                Item.NewItem(new EntitySource_TileBreak(i, j), new Vector2(i, j) * 16f, ModContent.ItemType<SpookySeedsGreen>());
            }
        }
    }

    public class SpookyWeedBig4 : SpookyWeedBig3
    {
    }
}