using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb
{
    public class CatacombBrickWall2UnsafeItem : ModItem
    {
		public override string Texture => "Spooky/Content/Tiles/Catacomb/CatacombBrickWall2Item";

		public override void SetStaticDefaults()
        {
			ItemID.Sets.DrawUnsafeIndicator[Type] = true;
			Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<CatacombBrickWall2>());
            Item.width = 16;
			Item.height = 16;
        }
    }
}