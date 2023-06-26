using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Buffs.Debuff;
using Spooky.Content.Items.SpookyHell.Misc;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoreHoodEye : ModItem
	{
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
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.GoreArmor");
			player.GetModPlayer<SpookyPlayer>().GoreArmorSet = true;

			if (!player.HasBuff(ModContent.BuffType<GoreAuraCooldown>()))
			{
				player.AddBuff(ModContent.BuffType<GoreAuraBuff>(), 1);
			}
		}

		public override void ArmorSetShadows(Player player)
		{
			player.armorEffectDrawShadow = true;
			player.armorEffectDrawOutlines = true;
		}

		public override void UpdateEquip(Player player) 
		{
			player.GetDamage(DamageClass.Magic) += 0.20f;
			player.GetDamage(DamageClass.Summon) += 0.20f;
			player.GetCritChance(DamageClass.Magic) += 10;
			player.GetCritChance(DamageClass.Summon) += 10;
			player.manaCost -= 0.10f;
			player.maxMinions += 3;
			player.aggro += 75;
		}

		public override void AddRecipes()
        {
            CreateRecipe()
            .AddIngredient(ModContent.ItemType<ArteryPiece>(), 12)
			.AddIngredient(ModContent.ItemType<CreepyChunk>(), 15)
            .AddTile(TileID.MythrilAnvil)
            .Register();
        }
	}
}
