using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Minibiomes.Christmas
{
    public class ChristmasBrickRedWallUnsafeItem : ModItem
    {
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickRedWallItem";

		public override void SetStaticDefaults()
        {
			ItemID.Sets.DrawUnsafeIndicator[Type] = true;
			Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<ChristmasBrickRedWall>());
            Item.width = 16;
			Item.height = 16;
        }
	}

    public class ChristmasBrickBlueWallUnsafeItem : ModItem
    {
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickBlueWallItem";

		public override void SetStaticDefaults()
        {
			ItemID.Sets.DrawUnsafeIndicator[Type] = true;
			Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<ChristmasBrickBlueWall>());
            Item.width = 16;
			Item.height = 16;
        }
	}

    public class ChristmasBrickGreenWallUnsafeItem : ModItem
    {
		public override string Texture => "Spooky/Content/Tiles/Minibiomes/Christmas/ChristmasBrickGreenWallItem";

		public override void SetStaticDefaults()
        {
			ItemID.Sets.DrawUnsafeIndicator[Type] = true;
			Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<ChristmasBrickGreenWall>());
            Item.width = 16;
			Item.height = 16;
        }
	}
}