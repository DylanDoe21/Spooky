using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class SpookyGravestone1 : ModTile
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
            AddMapEntry(new Color(12, 49, 10));
            DustType = DustID.Stone;
            HitSound = SoundID.Dig;
        }
    }

    public class SpookyGravestone2 : SpookyGravestone1
    {
    }

    public class SpookyGravestone3 : SpookyGravestone1
    {
    }
}