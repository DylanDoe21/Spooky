using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.SpookyBiome;

namespace Spooky.Content.Items.SpookyBiome.Boss
{
	public class PumpkinStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rot Spike Staff");
			Tooltip.SetDefault("Casts rotten spikes that will fall down rapidly midair"
			+ "\nThe spikes will deal more damage while falling down");
			Item.staff[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 15;           
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;       
			Item.width = 38;           
			Item.height = 44;         
			Item.useTime = 38;         
			Item.useAnimation = 38;         
			Item.useStyle = 5;          
			Item.knockBack = 6;
			Item.rare = 1;  
			Item.value = Item.buyPrice(silver: 50);
			Item.UseSound = SoundID.Item70;     
			Item.shoot = ModContent.ProjectileType<PumpkinStaffShard>();
			Item.shootSpeed = 12f;
		}

		/*
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
		*/

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
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
			
			return true;
		}
	}
}
