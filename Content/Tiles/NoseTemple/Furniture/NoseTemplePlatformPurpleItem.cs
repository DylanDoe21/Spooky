using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Tiles.NoseTemple.Furniture
{
	public class NoseTemplePlatformPurpleItem : ModItem
    {
		public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 200;
        }

		public override void SetDefaults() 
		{
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.consumable = true;
            Item.width = 16;
			Item.height = 16;
			Item.useTime = 10;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.maxStack = 9999;
			Item.createTile = ModContent.TileType<NoseTemplePlatformPurple>();
		}

		public override void AddRecipes()
        {
            CreateRecipe(2)
            .AddIngredient(ModContent.ItemType<NoseTempleFancyBrickPurpleItem>())
            .Register();
        }
	}
}