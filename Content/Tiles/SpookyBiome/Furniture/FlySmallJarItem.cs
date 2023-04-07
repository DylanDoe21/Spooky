using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpookyBiome;

namespace Spooky.Content.Tiles.SpookyBiome.Furniture
{
	public class FlySmallJarItem : ModItem
    {
		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 15;
			Item.useAnimation = 15;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<FlySmallJar>();
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.Bottle)
			.AddIngredient(ModContent.ItemType<FlySmallItem>())
            .Register();
        }
	}
}