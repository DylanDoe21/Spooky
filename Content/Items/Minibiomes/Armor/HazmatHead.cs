using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class HazmatHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 10;
			Item.width = 26;
			Item.height = 30;
			Item.rare = ItemRarityID.Pink;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<HazmatBody>() && legs.type == ModContent.ItemType<HazmatLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.HazmatArmor");
			//player.GetModPlayer<BloomBuffsPlayer>().HazmatSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
		}

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<SpookyWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}