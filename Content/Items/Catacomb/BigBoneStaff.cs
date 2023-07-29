using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.staff[Item.type] = true;
			ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<BigBoneScepter>();
		}

		public override void SetDefaults()
		{
			Item.damage = 100;  
			Item.mana = 30;         
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;  
			Item.noMelee = true;  
			Item.width = 66;           
			Item.height = 56;         
			Item.useTime = 30;         
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Yellow;
			Item.value = Item.buyPrice(gold: 25);
			Item.UseSound = SoundID.Item88;     
			Item.shoot = ModContent.ProjectileType<StaffFireBolt>();
			Item.shootSpeed = 15f;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2)
			{
				Item.UseSound = SoundID.DD2_ExplosiveTrapExplode;
				Item.shoot = ModContent.ProjectileType<FireBeam>();
			}
			else
			{
				Item.UseSound = SoundID.Item88;
				Item.shoot = ModContent.ProjectileType<StaffFireBolt>();
			}

			return true;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (player.altFunctionUse != 2)
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
					Vector2 newShootSpeed = new Vector2(mousePosX, mousePosY).SafeNormalize(Vector2.UnitY) * Item.shootSpeed;
                    newVelocity = newVelocity.SafeNormalize(newShootSpeed) * Item.shootSpeed;
                    newVelocity = Vector2.Lerp(newVelocity, newShootSpeed, 0.25f);
					Projectile.NewProjectile(source, randomPosition, newVelocity, type, damage, knockback, player.whoAmI, 0f, 0f);
				}
			}
			
			return true;
		}
	}
}
