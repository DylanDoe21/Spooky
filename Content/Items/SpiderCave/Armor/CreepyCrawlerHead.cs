using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class CreepyCrawlerHead : ModItem, IHelmetGlowmask
	{
		public string GlowmaskTexture => "Spooky/Content/Items/SpiderCave/Armor/CreepyCrawlerHead_Glow";

		public override void SetDefaults() 
		{
			Item.defense = 3;
			Item.width = 26;
			Item.height = 26;
			Item.rare = ItemRarityID.Blue;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<CreepyCrawlerBody>() && legs.type == ModContent.ItemType<CreepyCrawlerLegs>();
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.CreepyCrawlerArmor");
			player.GetModPlayer<SpookyPlayer>().CreepyCrawlerSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetDamage(DamageClass.Generic) += 0.1f;
			player.nightVision = true;
        }
	}
}