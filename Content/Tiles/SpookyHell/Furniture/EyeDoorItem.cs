using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.SpookyHell.Furniture
{
	public class EyeDoorItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<EyeDoorClosed>());
            Item.width = 16;
			Item.height = 16;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<LivingFleshItem>(), 6)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}