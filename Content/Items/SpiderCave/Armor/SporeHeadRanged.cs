using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SporeHeadRanged : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 24;
			Item.height = 32;
			Item.rare = ItemRarityID.LightRed;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SporeBody>() && legs.type == ModContent.ItemType<SporeLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			//player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SporeArmorRanged");
			//player.GetModPlayer<SpookyPlayer>().SporeRangedSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
        }

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<RootWoodItem>(), 20)
            .AddTile(TileID.WorkBenches)
            .Register();
        }
		*/
	}
}