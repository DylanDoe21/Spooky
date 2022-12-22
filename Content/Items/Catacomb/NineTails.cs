using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class NineTails : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cat O' Nine Tails");
			Tooltip.SetDefault("Your summons will focus struck enemies"
			+ "\nSwings out multiple short range whips at once"
			+ "\n'There is no safeword'");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 18;
			Item.DamageType = DamageClass.SummonMeleeSpeed;
			Item.noMelee = true;
			Item.noUseGraphic = true;
			Item.autoReuse = true;
			Item.width = 40;           
			Item.height = 60;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 2;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 2);
			Item.UseSound = SoundID.Item152;
			Item.shoot = ModContent.ProjectileType<Blank>();
			Item.shootSpeed = 3.5f;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
			{
				Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
				int i = Main.myPlayer;
				float num72 = Item.shootSpeed - Main.rand.Next(1, 3);
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
				vector14 = Vector2.Lerp(vector14, vector15, Main.rand.NextFloat(-0.25f, 0.25f));
				Projectile.NewProjectile(source, player.Center, vector14, ModContent.ProjectileType<NineTailsProj>(), num73, num74, i, 0f, 0f);
			}
			
			return true;
		}
	}
}
