using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Cemetery.Ambient
{
    public class DirtPile1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(65, 47, 40));
            DustType = DustID.Dirt;
            HitSound = SoundID.Dig;
        }
    }

    public class DirtPile2 : DirtPile1
    {
    }

    public class DirtPile3 : DirtPile1
    {
    }

    public class DirtPile4 : DirtPile1
    {
    }

    public class DirtPile5 : DirtPile1
    {
    }

    public class DirtPile6 : DirtPile1
    {
    }

    public class DirtPile7 : DirtPile1
    {
    }

    public class DirtPile8 : DirtPile1
    {
    }
}