using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Catacomb;

namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flame Orb Staff");
			/* Tooltip.SetDefault("Left click to cast a large spread of fire balls that bounce off of tiles"
			+ "\nRight click to unleash a high damage fire beam that targets enemies"); */
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
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
			Item.useTime = 35;         
			Item.useAnimation = 35;
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
					Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
					int i = Main.myPlayer;
					float num72 = Item.shootSpeed;
					int num73 = damage;
					float num74 = knockback;
					float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
					float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
					float f = Main.rand.NextFloat() * 6.28318548f;
					float value12 = 20f;
					float value13 = 60f;
					Vector2 vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
					
					for (int num202 = 0; num202 < 50; num202++)
					{
						vector13 = vector2 + f.ToRotationVector2() * MathHelper.Lerp(value12, value13, Main.rand.NextFloat());
						if (Collision.CanHit(vector2, 0, 0, vector13 + (vector13 - vector2).SafeNormalize(Vector2.UnitX) * 8f, 0, 0))
						{
							break;
						}

						f = Main.rand.NextFloat() * 6.28318548f;
					}

					Vector2 mouseWorld = Main.MouseWorld;
					Vector2 vector14 = mouseWorld - vector13;
					Vector2 vector15 = new Vector2(num78, num79).SafeNormalize(Vector2.UnitY) * num72;
					vector14 = vector14.SafeNormalize(vector15) * num72;
					vector14 = Vector2.Lerp(vector14, vector15, 0.25f);
					Projectile.NewProjectile(source, vector13, vector14, type, num73, num74, i, 0f, 0f);
				}
			}
			
			return true;
		}
	}
}
