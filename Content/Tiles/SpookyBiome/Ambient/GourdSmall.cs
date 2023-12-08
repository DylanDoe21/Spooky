using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    [LegacyName("SpookyPumpkin1")]
    [LegacyName("SpookyPumpkin2")]
    [LegacyName("SpookyPumpkin3")]
    public class GourdSmall1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.DrawYOffset = 6;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(73, 120, 46));
            DustType = 288;
            HitSound = SoundID.Dig;
        }
    }

    public class GourdSmall2 : GourdSmall1
    {
    }
}