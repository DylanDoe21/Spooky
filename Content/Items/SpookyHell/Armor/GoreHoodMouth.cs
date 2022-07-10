using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoreHoodMouth : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Gore Monger Brutal Hood");
			Tooltip.SetDefault("15% increased melee and ranged damage"
			+ "\n8% increased melee and ranged critical strike chance"
			+ "\n12% increased melee speed and 20% chance to not consume ammo"
			+ "\nEnemies are more likely to target you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 28;
			Item.height = 26;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 2);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<GoreBody>() && legs.type == ModContent.ItemType<GoreLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = "Grants you a protective aura that will block one attack"
			+ "\nWhile the aura is active, you gain 15% increased melee and ranged damage"
			+ "\nWhen you are hit the aura will vanish, and will regenerate after 45 seconds";
			player.GetModPlayer<SpookyPlayer>().GoreArmorSet = true;

			if (!player.HasBuff(ModContent.BuffType<GoreAuraCooldown>()))
            {
				player.AddBuff(ModContent.BuffType<GoreAuraBuff>(), 1);

				player.GetDamage(DamageClass.Melee) += 0.15f;
				player.GetDamage(DamageClass.Ranged) += 0.15f;
            }
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
			player.armorEffectDrawOutlines = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Melee) += 0.15f;
			player.GetDamage(DamageClass.Ranged) += 0.15f;
			player.GetCritChance(DamageClass.Melee) += 8;
			player.GetCritChance(DamageClass.Ranged) += 8;
			player.GetAttackSpeed(DamageClass.Melee) += 0.12f;
			player.ammoCost80 = true;
			player.aggro += 75;
		}
	}
}