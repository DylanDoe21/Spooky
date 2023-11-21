using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class NineTails : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.width = 40;           
			Item.height = 60;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Green;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 4f;
		}

		public override bool MeleePrefix() 
		{
			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			for (int numProjectiles = 0; numProjectiles < 4; numProjectiles++)
            {
                Vector2 mountedCenter = player.RotatedRelativePoint(player.MountedCenter, true);
                float mousePosX = Main.mouseX + Main.screenPosition.X - mountedCenter.X;
                float mousePosY = Main.mouseY + Main.screenPosition.Y - mountedCenter.Y;
                float random = Main.rand.NextFloat() * 6.28318548f;
                Vector2 randomPosition = mountedCenter + random.ToRotationVector2() * MathHelper.Lerp(20f, 60f, Main.rand.NextFloat());

                for (int numPositon = 0; numPositon < 50; numPositon++)
                {
                    randomPosition = mountedCenter + random.ToRotationVector2() * MathHelper.Lerp(20f, 60f, Main.rand.NextFloat());
                    if (Collision.CanHit(mountedCenter, 0, 0, randomPosition + (randomPosition - mountedCenter).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
                    {
                        break;
                    }

                    random = Main.rand.NextFloat() * 6.28318548f;
                }

                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 newVelocity = mouseWorld - randomPosition;
                Vector2 newShootSpeed = new Vector2(mousePosX, mousePosY).SafeNormalize(Vector2.UnitY) * (Item.shootSpeed - Main.rand.Next(2, 4));
                newVelocity = newVelocity.SafeNormalize(newShootSpeed) * (Item.shootSpeed - Main.rand.Next(2, 4));
                newVelocity = Vector2.Lerp(newVelocity, newShootSpeed, Main.rand.NextFloat(-0.25f, 0.25f));
                Projectile.NewProjectile(source, randomPosition, newVelocity, ModContent.ProjectileType<NineTailsProj>(), damage, knockback, player.whoAmI, 0f, 0f);
            }

            return true;
		}
	}
}
