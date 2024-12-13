using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb
{
    [LegacyName("CatacombBrickWall2SafeItem")]
    public class CatacombBrickWall2Item : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CatacombBrickWall2UnsafeItem>();
            Item.ResearchUnlockCount = 400;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableWall(ModContent.WallType<CatacombBrickWall2Safe>());
            Item.width = 16;
			Item.height = 16;
        }

        public override void AddRecipes()
        {
            CreateRecipe(4)
            .AddIngredient(ModContent.ItemType<CatacombBrick2Item>())
			.AddDecraftCondition(Condition.DownedGolem)
			.AddTile(TileID.WorkBenches)
            .Register();
        }
    }
}