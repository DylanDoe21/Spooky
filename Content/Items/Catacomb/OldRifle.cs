using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class OldRifle : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Old Hunter's Rifle");
			Tooltip.SetDefault("Shoots out rusty bullets that split on impact");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 130;
			Item.DamageType = DamageClass.Ranged;
			Item.noMelee = true;
			Item.autoReuse = true; 
			Item.width = 106;           
			Item.height = 30;
			Item.useTime = 55;         
			Item.useAnimation = 55;
			Item.useStyle = ItemUseStyleID.Shoot;         
			Item.knockBack = 5;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.buyPrice(gold: 15);
			Item.UseSound = SoundID.Item36;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 12f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-25, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 70f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

			for (int numExplosion = 0; numExplosion < 3; numExplosion++)
            {
				int DustGore = Dust.NewDust(position, player.width / 2, player.height / 2, ModContent.DustType<SmokeEffect>(), 
				0f, 0f, 100, new Color(146, 75, 19) * 0.5f, Main.rand.NextFloat(0.5f, 1f));

				Main.dust[DustGore].velocity.X *= 0.2f;
                Main.dust[DustGore].velocity.Y *= Main.rand.NextFloat(0f, 1f);
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

			Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<OldRifleBullet>(), damage, knockback, player.whoAmI, 0f, 0f);

			return false;
		}
	}
}
