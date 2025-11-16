using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.Catacomb.Furniture
{
	public class UpperCatacombPianoItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<UpperCatacombPiano>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<CatacombBrick1Item>(), 15)
			.AddIngredient(ItemID.Bone, 4)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}