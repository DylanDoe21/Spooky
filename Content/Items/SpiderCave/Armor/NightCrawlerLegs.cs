using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class NightCrawlerLegs : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 12;
			Item.width = 22;
			Item.height = 16;
			Item.rare = ItemRarityID.Pink;
		}

		public override void UpdateEquip(Player player) 
		{
            player.moveSpeed += 0.3f;
            player.runAcceleration += 0.05f;
            player.jumpBoost = true;
        }

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ModContent.ItemType<CreepyCrawlerLegs>())
			.AddIngredient(ItemID.SoulofNight, 10)
			.AddIngredient(ItemID.HallowedBar, 8)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
		*/
	}
}