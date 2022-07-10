using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Tiles.SpookyHell.Ambient
{
    public class Tooth1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(57, 60, 115));
            DustType = DustID.PurpleCrystalShard;
            HitSound = SoundID.NPCHit13;
        }
    }

    public class Tooth2 : Tooth1
    {
    }

    public class Tooth3 : ModTile
    {
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLighted[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
            TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(57, 60, 115));
            DustType = DustID.PurpleCrystalShard;
			HitSound = SoundID.NPCHit13;
		}
    }

    public class Tooth4 : ModTile
    {
		public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(57, 60, 115));
            DustType = DustID.PurpleCrystalShard;
            HitSound = SoundID.NPCHit13;
        }
	}
}