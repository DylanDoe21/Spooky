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

			if (player.ownedProjectileCounts[ModContent.ProjectileType<MiniOrroHead>()] <= 0)
			{
				SpawnWorm(ModContent.ProjectileType<MiniOrroHead>(), ModContent.ProjectileType<MiniOrroBody>(), ModContent.ProjectileType<MiniOrroTail>(), new Vector2(player.Center.X, player.Center.Y - 50), player, 85, 0);
			}
		}

        public static void SpawnWorm(int head, int body, int tail, Vector2 spawnPos, Player player, int damage, float knockback)
        {
            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, head, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(null, spawnPos, Vector2.Zero, tail, damage, knockback, player.whoAmI);

            for (var i = 0; i < 5; i++)
            {
                Projectile.NewProjectile(null, spawnPos, Vector2.Zero, body, damage, knockback, player.whoAmI);
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
