using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class CoralGreen1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(72, 214, 131));
            HitSound = SoundID.Dig;
            DustType = -1;
        }
    }

    public class CoralGreen2 : CoralGreen1
    {
    }

    public class CoralGreen3 : CoralGreen1
    {
    }

    public class CoralPurple1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(156, 72, 214));
            HitSound = SoundID.Dig;
            DustType = -1;
        }
    }

    public class CoralPurple2 : CoralPurple1
    {
    }

    public class CoralPurple3 : CoralPurple1
    {
    }

    public class CoralRed1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(214, 82, 72));
            HitSound = SoundID.Dig;
            DustType = -1;
        }
    }

    public class CoralRed2 : CoralRed1
    {
    }

    public class CoralRed3 : CoralRed1
    {
    }

    public class CoralYellow1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(214, 156, 72));
            HitSound = SoundID.Dig;
            DustType = -1;
        }
    }

    public class CoralYellow2 : CoralYellow1
    {
    }

    public class CoralYellow3 : CoralYellow1
    {
    }
}