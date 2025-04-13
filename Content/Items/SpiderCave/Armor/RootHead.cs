using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class RootHead : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.White;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<RootBody>() && legs.type == ModContent.ItemType<RootLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.RootArmor");
			player.GetModPlayer<SpookyPlayer>().RootSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetCritChance(DamageClass.Ranged) += 5;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RootWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
	}
}