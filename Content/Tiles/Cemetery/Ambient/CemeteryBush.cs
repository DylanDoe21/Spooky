using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Tiles.Cemetery.Ambient
{
    public class CemeteryBush1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			Main.tileSolid[Type] = false;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.addTile(Type);
            TileObjectData.newTile.DrawYOffset = 2;
            AddMapEntry(new Color(31, 94, 61));
            DustType = ModContent.DustType<CemeteryGrassDust>();
			HitSound = SoundID.Grass;
        }
    }

    public class CemeteryBush2 : CemeteryBush1
    {
    }
}