using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class BroccoliLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
			player.lifeRegen += 1;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 28)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}