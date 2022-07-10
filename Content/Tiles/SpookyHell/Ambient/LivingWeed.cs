using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
    public class LivingWeed1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(88, 49, 129));
            DustType = DustID.PurpleCrystalShard;
            HitSound = SoundID.NPCHit13;
        }
    }

    public class LivingWeed2 : LivingWeed1
    {
    }

    public class LivingWeed3 : ModTile
    {
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(52, 40, 101));
            DustType = DustID.PurpleCrystalShard;
			HitSound = SoundID.NPCHit13;
		}
	}

    public class LivingWeed4 : LivingWeed3
    {
    }

    public class LivingWeed6 : LivingWeed3
    {
    }

    public class LivingWeed5 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(33, 26, 62));
            DustType = DustID.PurpleCrystalShard;
            HitSound = SoundID.NPCHit13;
        }
    }
}