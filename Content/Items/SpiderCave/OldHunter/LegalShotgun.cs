using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;
 
namespace Spooky.Content.Items.SpiderCave.OldHunter
{
	public class LegalShotgun : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 18;
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

		//this dont work lol
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 70f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
            {
                position += muzzleOffset;
            }

            for (int numProjectiles = 0; numProjectiles < 8; numProjectiles++)
            {
                Vector2 randomPosition = position * MathHelper.Lerp(20f, 60f, Main.rand.NextFloat());

                Vector2 newVelocity = Main.MouseWorld - randomPosition;
                Vector2 newShootSpeed = Main.MouseWorld.SafeNormalize(Vector2.UnitY) * (Item.shootSpeed - Main.rand.Next(2, 4));
                newVelocity = newVelocity.SafeNormalize(newShootSpeed) * (Item.shootSpeed - Main.rand.Next(2, 4));
                newVelocity = Vector2.Lerp(newVelocity, newShootSpeed, Main.rand.NextFloat(-0.25f, 0.25f));
                Projectile.NewProjectile(source, randomPosition, newVelocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            }

            player.velocity -= velocity;

			return false;
		}
	}
}