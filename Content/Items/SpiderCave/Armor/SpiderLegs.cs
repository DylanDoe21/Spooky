using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class SpiderLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 2;
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
			.AddIngredient(ItemID.Silk, 25)
			.AddIngredient(ItemID.SilverBar, 16)
            .AddIngredient(ModContent.ItemType<WebBlockItem>(), 70)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.Silk, 25)
			.AddIngredient(ItemID.TungstenBar, 16)
            .AddIngredient(ModContent.ItemType<WebBlockItem>(), 70)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}