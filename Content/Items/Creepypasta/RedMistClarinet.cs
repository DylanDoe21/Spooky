using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Projectiles.Creepypasta;

namespace Spooky.Content.Items.Creepypasta
{
	public class RedMistClarinet : ModItem
	{
		public static readonly SoundStyle ClarinetSound = new("Spooky/Content/Sounds/Clarinet", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Clarinet");
			Tooltip.SetDefault("Playing it will fire a spread of short lived blood notes"
			+ "\nBlood notes will turn into a lingering red mist cloud on death");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 12;  
			Item.mana = 20;         
			Item.DamageType = DamageClass.Magic;
			Item.autoReuse = true;       
			Item.width = 48;           
			Item.height = 22;         
			Item.useTime = 38;         
			Item.useAnimation = 38;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 4;
			Item.rare = ItemRarityID.Blue;
			Item.value = Item.buyPrice(gold: 1);
			Item.UseSound = ClarinetSound;     
			Item.shoot = ModContent.ProjectileType<RedMistNote1>();
			Item.shootSpeed = 6f;
		}

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

			int[] Type = new int[] { ModContent.ProjectileType<RedMistNote1>(), ModContent.ProjectileType<RedMistNote2>() };

			Projectile.NewProjectile(source, vector13, vector14, Main.rand.Next(Type), num73, num74, i, 0f, 0f);
			
			return true;
		}
	}
}
