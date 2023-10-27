using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.SpiderCave.Ambient
{
    public class MushroomBlue1 : ModTile
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
            AddMapEntry(new Color(78, 58, 197));
            HitSound = SoundID.Dig;
        }
    }

    public class MushroomBlue2 : MushroomBlue1
    {
    }

    public class MushroomBlue3 : MushroomBlue1
    {
    }

    public class MushroomBlue4 : MushroomBlue1
    {
    }
}