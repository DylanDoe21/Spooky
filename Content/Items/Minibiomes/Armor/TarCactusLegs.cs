using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class TarCactusLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 22;
			Item.height = 16;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<TarPitCactusBlockItem>(), 25)
            .AddRecipeGroup(RecipeGroupID.IronBar, 15)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}