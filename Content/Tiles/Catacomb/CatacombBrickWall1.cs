using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
    [LegacyName("CatacombBrickWallDaffodilBG")]
    public class CatacombBrickWall1 : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            Main.wallBlend[Type] = ModContent.WallType<CatacombGrassWall1>();
            AddMapEntry(new Color(29, 24, 35));
			RegisterItemDrop(ModContent.ItemType<CatacombBrickWall1Item>());
			DustType = DustID.Stone;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = !Flags.downedDaffodil;
        }
    }

	public class CatacombBrickWall1Safe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall1";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(29, 24, 35));
            DustType = DustID.Stone;
        }
    }
}