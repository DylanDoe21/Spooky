using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyBiome.Ambient
{
    public class GourdGiant1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.DrawYOffset = 4;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(147, 33, 27));
            DustType = 288;
            HitSound = SoundID.Dig;
        }
    }

    public class GourdGiant2 : GourdGiant1
    {
    }
}