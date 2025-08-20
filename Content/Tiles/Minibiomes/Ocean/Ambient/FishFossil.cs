using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class FishFossil1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(127, 130, 98));
            DustType = DustID.Bone;
            HitSound = SoundID.Dig;

        }
    }

    public class FishFossil2 : FishFossil1
    {
    }

    public class FishFossil3 : FishFossil1
    {
    }

    public class FishFossil4 : FishFossil1
    {
    }

    public class FishFossil5 : FishFossil1
    {
    }
}