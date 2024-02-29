using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
    public class Bone1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(130, 130, 130));
            DustType = DustID.Bone;
            HitSound = SoundID.DD2_SkeletonHurt;
        }
    }

    public class Bone2 : Bone1
    {
    }

    public class Bone3 : Bone1
    {
    }

    public class Bone4 : Bone1
    {
    }
}