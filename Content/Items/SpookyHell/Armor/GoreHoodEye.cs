using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell.Armor
{
	[AutoloadEquip(EquipType.Head)]
	public class GoreHoodEye : ModItem
	{
		public override void SetDefaults() 
		{
			Item.defense = 10;
			Item.width = 34;
			Item.height = 30;
			Item.rare = ItemRarityID.LightPurple;
			Item.value = Item.buyPrice(gold: 4);
		}

		public override bool IsArmorSet(Item head, Item body, Item legs) 
		{
			return body.type == ModContent.ItemType<GoreBody>() && legs.type == ModContent.ItemType<GoreLegs>();
		}
		
		public override void UpdateArmorSet(Player player) 
		{
			player.setBonus = Language.GetTextValue("Mods.Spooky.ArmorSetBonus.GoreArmorOrro");

			player.GetModPlayer<SpookyPlayer>().GoreArmorEye = true;

			if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniOrroHead>()] <= 0 && player.ownedProjectileCounts[ModContent.ProjectileType<MiniBoroHead>()] <= 0)
			{
				Projectile.NewProjectile(null, player.Center, Vector2.Zero, ModContent.ProjectileType<MiniOrroHead>(), 85, 0, player.whoAmI);
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
			player.manaCost -= 0.10f;
			player.maxMinions += 2;
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
