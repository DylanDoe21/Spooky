using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class CreepyCrawlerBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
			Item.width = 46;
			Item.height = 20;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetModPlayer<SpookyPlayer>().CreepyCrawlerSpeed = true;
            player.GetCritChance(DamageClass.Generic) += 3;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.Silk, 30)
			.AddIngredient(ItemID.SilverBar, 22)
            .AddIngredient(ModContent.ItemType<WebBlockItem>(), 80)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.Silk, 30)
			.AddIngredient(ItemID.TungstenBar, 22)
            .AddIngredient(ModContent.ItemType<WebBlockItem>(), 80)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}