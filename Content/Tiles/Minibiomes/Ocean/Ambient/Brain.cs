using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.Minibiomes.Ocean.Ambient
{
    public class Brain1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(236, 133, 138));
            HitSound = SoundID.Dig;
        }
    }

    public class Brain2 : Brain1
    {
    }

    public class Brain3 : Brain1
    {
    }

    public class Brain4 : Brain1
    {
    }
}