using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Body)]
	public class NightCrawlerBody : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 15;
			Item.width = 46;
			Item.height = 22;
			Item.rare = ItemRarityID.Pink;
		}

        public override void UpdateEquip(Player player)
        {
            player.GetModPlayer<SpookyPlayer>().NightCrawlerSpeed = true;
            player.GetCritChance(DamageClass.Generic) += 10;
        }

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ModContent.ItemType<CreepyCrawlerBody>())
			.AddIngredient(ItemID.SoulofNight, 12)
			.AddIngredient(ItemID.HallowedBar, 10)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
		*/
    }
}