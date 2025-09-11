using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Minibiomes.Christmas;
 
namespace Spooky.Content.Items.Minibiomes.Christmas
{
	public class SnakeTrombone : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.mana = 12;
			Item.DamageType = DamageClass.Magic;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.width = 64;
			Item.height = 30;
			Item.useTime = 35;
			Item.useAnimation = 35;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 1;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item42;
			Item.shoot = ModContent.ProjectileType<SnakeHead>();
			Item.shootSpeed = 8f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(8));

			for (int numDusts = 0; numDusts < 12; numDusts++)
			{
				Dust dust = Dust.NewDustPerfect(position, DustID.Shadowflame, new Vector2(newVelocity.X + Main.rand.Next(-7, 8), newVelocity.Y + Main.rand.Next(-7, 8)) * 0.5f, default, default, 2f);
				dust.noGravity = true;
				dust.velocity += player.velocity;
			}

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, player.whoAmI);
			
			return false;
		}
	}
}
