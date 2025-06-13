using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Ocean;
 
namespace Spooky.Content.Items.Minibiomes.Ocean
{
	public class BloodSoakerBig : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 45;
			Item.mana = 15;
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;
			Item.noMelee = true;
			Item.width = 88;
			Item.height = 42;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 10);
			Item.UseSound = SoundID.Item171 with { Pitch = -0.5f, Volume = 0.5f };
			Item.shoot = ModContent.ProjectileType<BloodGunSplatter>();
			Item.shootSpeed = 15f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 80f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			for (int numProjectiles = 0; numProjectiles < 5; numProjectiles++)
			{
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));

				Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}
			
			return false;
		}
	}
}