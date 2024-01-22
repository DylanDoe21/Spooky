using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class SpiderBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 4;
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