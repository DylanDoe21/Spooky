using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.Cemetery.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SpiritHorsemanLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 22;
			Item.height = 18;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override void UpdateEquip(Player player) 
		{
			player.moveSpeed += 0.12f;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyPlasma>(), 12)
			.AddIngredient(ItemID.Silk, 15)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}