using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SpiderLegs : ModItem
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
            player.moveSpeed += 0.15f;
            player.runAcceleration += 0.05f;
            player.jumpBoost = true;
        }

		public override void AddRecipes()
        {
			CreateRecipe()
			.AddIngredient(ItemID.SilverBar, 15)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 25)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 100)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.TungstenBar, 15)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 25)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 100)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}