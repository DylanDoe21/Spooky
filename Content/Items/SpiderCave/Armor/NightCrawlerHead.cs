using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class NightCrawlerHead : ModItem, IHelmetGlowmask
	{
		public string GlowmaskTexture => "Spooky/Content/Items/SpiderCave/Armor/NightCrawlerHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 28;
			Item.height = 26;
			Item.rare = ItemRarityID.Pink;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<NightCrawlerBody>() && legs.type == ModContent.ItemType<NightCrawlerLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.NightCrawlerArmor");
			player.GetModPlayer<SpookyPlayer>().NightCrawlerSet = true;
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetDamage(DamageClass.Generic) += 0.18f;
            player.nightVision = true;
			player.dangerSense = true;
        }

		/*
		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ModContent.ItemType<CreepyCrawlerHead>())
			.AddIngredient(ItemID.SoulofNight, 8)
			.AddIngredient(ItemID.HallowedBar, 5)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
		*/
	}
}