using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Tiles.SpiderCave;

namespace Spooky.Content.Items.SpiderCave.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class MortarHead : ModItem
	{
		public override void SetStaticDefaults()
        {
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MortarBody>();
		}

		public override void SetDefaults() 
		{
			Item.defense = 20;
			Item.width = 18;
			Item.height = 22;
			Item.rare = ItemRarityID.Yellow;
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<MortarBody>() && legs.type == ModContent.ItemType<MortarLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.MortarArmor");
			player.GetModPlayer<SpookyPlayer>().MortarSet = true;
		}

		public override void UpdateEquip(Player player) 
		{
            player.GetCritChance(DamageClass.Generic) += 10;
			player.manaCost -= 0.08f;
			player.endurance += 0.02f;
        }
	}
}