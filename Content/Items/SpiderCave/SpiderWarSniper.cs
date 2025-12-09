using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Projectiles.SpiderCave;
using Spooky.Content.Tiles.SpiderCave;
 
namespace Spooky.Content.Items.SpiderCave
{
	public class SpiderWarSniper : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 265;
			Item.crit = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 76;
			Item.height = 26;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 3;
			Item.rare = ItemRarityID.Yellow;
            Item.value = Item.buyPrice(gold: 50);
			Item.UseSound = SoundID.Item98;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 15f;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-10, -2);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 65f;

			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
			{
				position += muzzleOffset;
			}

			//right click shoots a spread of purple bolts
			if (player.altFunctionUse == 2)
			{
				for (int numProjs = 0; numProjs < 5; numProjs++)
				{
					Vector2 newVelocity = new Vector2(velocity.X / Main.rand.NextFloat(2f, 3f), velocity.Y / Main.rand.NextFloat(2f, 3f)).RotatedByRandom(MathHelper.ToRadians(25));

					Projectile.NewProjectile(source, position, newVelocity, ModContent.ProjectileType<SniperBoltPurple>(), damage / 3, knockback, player.whoAmI, ai0: Main.rand.Next(0, 9));
				}
			}
			//left click shoots a fast 
			else
			{
				Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<SniperBoltGreen>(), damage, knockback, player.whoAmI);
			}

			return false;
		}
	}
}
