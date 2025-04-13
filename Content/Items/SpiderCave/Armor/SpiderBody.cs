using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpiderBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 5;
			Item.width = 30;
			Item.height = 18;
			Item.rare = ItemRarityID.Blue;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetModPlayer<SpookyPlayer>().SpiderSpeed = true;
            player.GetCritChance(DamageClass.Generic) += 3;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.SilverBar, 25)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 25)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 180)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.TungstenBar, 25)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 25)
			.AddIngredient(ModContent.ItemType<WebBlockItem>(), 180)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}