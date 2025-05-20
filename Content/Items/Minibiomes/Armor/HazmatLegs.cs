using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class HazmatLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Pink;
		}

		public override void UpdateEquip(Player player) 
		{
		}

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.HallowedBar, 12)
            .AddIngredient(ModContent.ItemType<PlantMulch>(), 45)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}