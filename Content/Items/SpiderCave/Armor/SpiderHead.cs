using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class SpiderHead : ModItem, IHelmetGlowmask
	{
		public string GlowmaskTexture => "Spooky/Content/Items/SpiderCave/Armor/SpiderHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 28;
			Item.height = 22;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<SpiderBody>() && legs.type == ModContent.ItemType<SpiderLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.SpiderArmor");
			player.GetModPlayer<SpookyPlayer>().SpiderSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetDamage(DamageClass.Generic) += 0.1f;
			player.nightVision = true;
        }

		public override void AddRecipes()
        {
            CreateRecipe()
			.AddIngredient(ItemID.Silk, 20)
			.AddIngredient(ItemID.SilverBar, 8)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 20)
            .AddIngredient(ModContent.ItemType<WebBlockItem>(), 60)
            .AddTile(TileID.Anvils)
            .Register();

			CreateRecipe()
			.AddIngredient(ItemID.Silk, 20)
			.AddIngredient(ItemID.TungstenBar, 8)
			.AddIngredient(ModContent.ItemType<SpiderChitin>(), 20)
            .AddIngredient(ModContent.ItemType<WebBlockItem>(), 60)
            .AddTile(TileID.Anvils)
            .Register();
        }
	}
}