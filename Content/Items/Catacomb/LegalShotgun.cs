using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
 
namespace Spooky.Content.Items.Catacomb
{
	public class LegalShotgun : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 25;
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
			Item.value = Item.buyPrice(gold: 20);
			Item.UseSound = SoundID.Item36;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.useAmmo = AmmoID.Bullet;
			Item.shootSpeed = 12f;
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 35f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
            {
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(25));

			    Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            player.velocity -= velocity / 2;

			return false;
		}
	}
}