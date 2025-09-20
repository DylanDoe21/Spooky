using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class LittleSpiderJarItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LittleSpiderJar>());
            Item.width = 16;
			Item.height = 16;
		}

        
        public override void AddRecipes()
        {
            
            CreateRecipe()
            .AddIngredient(ItemID.Bottle)
            .AddIngredient(ModContent.ItemType<LittleSpiderItem>())
            .Register();
            
        }
        
	}
}