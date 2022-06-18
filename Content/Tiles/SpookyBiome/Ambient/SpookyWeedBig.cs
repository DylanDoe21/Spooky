using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyWeedBig1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileCut[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);   
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(30, 68, 27));
            DustType = DustID.Grass;
            HitSound = SoundID.Grass;
        }
    }

    public class SpookyWeedBig2 : SpookyWeedBig1
    {
    }

    public class SpookyWeedBig3 : SpookyWeedBig1
    {
    }
}