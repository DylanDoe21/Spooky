using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Tiles.Catacomb
{
    [LegacyName("CatacombBrickWallBigBoneBG")]
    public class CatacombBrickWall2 : ModWall 
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(44, 15, 15));
			RegisterItemDrop(ModContent.ItemType<CatacombBrickWall2Item>());
			DustType = DustID.t_Lihzahrd;
        }

        public override bool CanExplode(int i, int j)
		{
			return false;
		}

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = !Flags.downedBigBone;
        }
    }

	public class CatacombBrickWall2Safe : ModWall 
    {
        public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall2";

        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            AddMapEntry(new Color(44, 15, 15));
            DustType = DustID.t_Lihzahrd;
        }
    }
}