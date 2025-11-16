using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;
using Spooky.Content.Projectiles.Minibiomes.Vegetable;

namespace Spooky.Content.Items.Minibiomes.Vegetable
{
    public class GhostPepperStaff : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 30;
			Item.mana = 20;
            Item.DamageType = DamageClass.Summon;
			Item.noMelee = true;
			Item.autoReuse = true; 
            Item.width = 44;
            Item.height = 78;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.buyPrice(gold: 5);
            Item.UseSound = SoundID.NPCDeath52 with { Volume = 0.5f, Pitch = 1f };
            Item.buffType = ModContent.BuffType<GhostPepperMinionBuff>();
			Item.shoot = ModContent.ProjectileType<GhostPepperMinionTier1>();
            Item.shootSpeed = 7f;
        }

		public void FindAndUpgradeMinion(EntitySource_ItemUse_WithAmmo source, Player player, int TypeToCheckFor, int TypeToSpawn)
		{
			if (!CanSpawnMinion(player))
			{
				return;
			}
			else
			{
				foreach (Projectile proj in Main.ActiveProjectiles)
				{
					if (proj.type == TypeToCheckFor && proj.owner == player.whoAmI)
					{
						for (int numDusts = 0; numDusts < 25; numDusts++)
						{
							int DustGore = Dust.NewDust(proj.position, proj.width, proj.height, DustID.Ghost, 0f, -2f, 0, default, 1.5f);
							Main.dust[DustGore].noGravity = true;
							Main.dust[DustGore].velocity = proj.velocity;
							Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
							Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
						}

						var projectile = Projectile.NewProjectileDirect(source, proj.Center, proj.velocity, TypeToSpawn, (int)(proj.damage * 1.025f), proj.knockBack, player.whoAmI);
						projectile.originalDamage = (int)(proj.damage * 1.025f);

						proj.Kill();

						break;
					}
				}
			}
		}

		public bool CanSpawnMinion(Player player)
		{
			float foundSlotsCount = 0;
			foreach (Projectile proj in Main.ActiveProjectiles)
			{
				if (proj.minion && proj.owner == player.whoAmI)
				{
					foundSlotsCount += proj.minionSlots;
					if (foundSlotsCount > player.maxMinions - 1)
					{
						return false;
					}
				}
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			//if there is no ghost minion, just spawn it
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier1>()] <= 0 &&
			player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier2>()] <= 0 &&
			player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier3>()] <= 0 &&
			player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier4>()] <= 0)
			{
				var projectile  = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
				projectile.originalDamage = Item.damage;
			}
			//upgrade to tier 2, check for one minion slot only since the existing ghost pepper will be taking up slots equal to one less than its upgrade
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier1>()] > 0)
			{
				FindAndUpgradeMinion(source, player, ModContent.ProjectileType<GhostPepperMinionTier1>(), ModContent.ProjectileType<GhostPepperMinionTier2>());
			}
			//upgrade to tier 3
			else if (player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier2>()] > 0)
			{
				FindAndUpgradeMinion(source, player, ModContent.ProjectileType<GhostPepperMinionTier2>(), ModContent.ProjectileType<GhostPepperMinionTier3>());
			}
			//upgrade to tier 4
			else if (player.ownedProjectileCounts[ModContent.ProjectileType<GhostPepperMinionTier3>()] > 0)
			{
				FindAndUpgradeMinion(source, player, ModContent.ProjectileType<GhostPepperMinionTier3>(), ModContent.ProjectileType<GhostPepperMinionTier4>());
			}

			return false;
		}
    }
}