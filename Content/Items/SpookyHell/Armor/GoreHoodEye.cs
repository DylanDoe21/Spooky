using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.SpookyHell.Boss;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoreHoodEye : ModItem
	{
		public override void SetStaticDefaults() 
		{
			DisplayName.SetDefault("Gore Monger's Seer Hood");
			Tooltip.SetDefault("15% increased magic and summon damage"
			+ "\n8% increased magic and summon critical strike chance"
			+ "\n12% reduced mana usage and increases your max minions by 2"
			+ "\nEnemies are more likely to target you");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() 
		{
			Item.defense = 8;
			Item.width = 30;
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
			+ "\nWhile the aura is active, you gain 15% increased magic and summon damage"
			+ "\nWhen you are hit the aura will vanish, and will regenerate after 45 seconds";
			player.GetModPlayer<SpookyPlayer>().GoreArmorSet = true;

			if (!player.HasBuff(ModContent.BuffType<GoreAuraCooldown>()))
			{
				player.AddBuff(ModContent.BuffType<GoreAuraBuff>(), 1);

				player.GetDamage(DamageClass.Magic) += 0.15f;
				player.GetDamage(DamageClass.Summon) += 0.15f;
				player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.15f;
			}
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
			player.armorEffectDrawOutlines = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic) += 0.15f;
			player.GetDamage(DamageClass.Summon) += 0.15f;
			player.GetDamage(DamageClass.SummonMeleeSpeed) += 0.15f;
			player.GetCritChance(DamageClass.Magic) += 8;
			player.GetCritChance(DamageClass.Summon) += 8;
			player.manaCost -= 0.12f;
			player.maxMinions += 2;
			player.aggro += 75;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<OrroboroChunk>(), 12)
			.AddIngredient(ModContent.ItemType<EyeBlockItem>(), 45)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
