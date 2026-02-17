using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.Minibiomes.Desert;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class TarCactusBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 5;
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
            .AddIngredient(ModContent.ItemType<TarPitCactusBlockItem>(), 32)
            .AddRecipeGroup(RecipeGroupID.IronBar, 18)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}