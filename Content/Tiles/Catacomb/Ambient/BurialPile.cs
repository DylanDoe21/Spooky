using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Catacomb.Ambient
{
    public class BurialPile1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(93, 62, 39));
            DustType = DustID.Dirt;
            HitSound = SoundID.Dig;
        }
    }

    public class BurialPile2 : BurialPile1
    {
    }

    public class BurialPile3 : BurialPile1
    {
    }
}