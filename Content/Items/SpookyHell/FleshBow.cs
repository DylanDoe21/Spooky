using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.SpookyHell;

namespace Spooky.Content.Items.SpookyHell
{
	public class FleshBow : ModItem
	{
		int numUses = 0;

		public override void SetDefaults()
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true;
			Item.channel = true;
			Item.width = 36;
			Item.height = 60;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.knockBack = 2;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 8;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 3);
			Item.UseSound = SoundID.Item17;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.useAmmo = AmmoID.Arrow;
			Item.shootSpeed = 10f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-3, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            if (numUses >= 10)
			{
				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<BowEye>(), damage, knockback, player.whoAmI, 0f, 0f);

				numUses = 0;
			}
			else
			{
				int[] Types = new int[] { ModContent.ProjectileType<FleshBowChunk1>(), ModContent.ProjectileType<FleshBowChunk2>() };

				Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, Main.rand.Next(Types), damage, knockback, player.whoAmI, 0f, 0f);
			}
			
			numUses++;
			
			return false;
        }
	}
}