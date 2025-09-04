using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
 
using Spooky.Content.Projectiles.Minibiomes.Ocean;

namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class BloodSoaker : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.mana = 10;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 76;
			Item.height = 38;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1, silver: 50);
			Item.UseSound = SoundID.Item171 with { Pitch = -0.5f, Volume = 0.5f };
			Item.shoot = ModContent.ProjectileType<BloodGunSplatter>();
			Item.shootSpeed = 10f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-12, -5);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 62f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			for (int numProjectiles = 0; numProjectiles < 2; numProjectiles++)
			{
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));

				Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}
			
			return false;
		}
	}
}