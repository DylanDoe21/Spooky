using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;
using Spooky.Content.Projectiles.Catacomb;
 
namespace Spooky.Content.Items.Catacomb
{
	public class BigBoneHammer : SwingWeaponBase
	{
		public override int Length => 55;
		public override int TopSize => 25;
		public override float SwingDownSpeed => Main.LocalPlayer.altFunctionUse == 2 ? 1f : 12f;
		public override bool CollideWithTiles => true;
		static bool hasHitSomething = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Smasher");
			Tooltip.SetDefault("Left click to swing the hammer and create explosions on enemy hits"
			+ "\nHold down right click to swing the hammer around you and charge it up" 
			+ "\nOnce fully charged, releasing right click will throw the hammer");
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 250; 
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = false;
			Item.noUseGraphic = false;
			Item.width = 82;           
			Item.height = 76;
			Item.useTime = 75;
			Item.useAnimation = 75;
			Item.useStyle = SwingUseStyle;
			Item.knockBack = 12;
			Item.rare = ItemRarityID.Yellow;  
			Item.value = Item.buyPrice(gold: 25);
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
		}

		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			for (int i = 0; i < 1000; i++)
			{
				if (Main.projectile[i].active && (Main.projectile[i].type == ModContent.ProjectileType<BigBoneHammerProj>() ||
				Main.projectile[i].type == ModContent.ProjectileType<BigBoneHammerProj2>()))
				{
					return false;
				}
			}

			return true;
		}

		public override void UseAnimation(Player player)
		{
			hasHitSomething = false;

			if (player.altFunctionUse == 2)
			{
				Item.noMelee = true;
				Item.noUseGraphic = true;
				Item.autoReuse = true;
				Item.useTime = 45;
				Item.useAnimation = 45;
				Item.useStyle = SwingUseStyle;
				Item.UseSound = SoundID.DD2_MonkStaffSwing;
				Item.shoot = ModContent.ProjectileType<BigBoneHammerProj>();
				Item.shootSpeed = 10f;
			}
			else
			{
				Item.noMelee = false;
				Item.noUseGraphic = false;
				Item.autoReuse = true;
				Item.useTime = 75;
				Item.useAnimation = 75;
				Item.useStyle = SwingUseStyle;
				Item.UseSound = SoundID.DD2_MonkStaffSwing;
				Item.shoot = 0;
				Item.shootSpeed = 0f;
			}
		}

		public override void OnHitTiles(Player player)
		{
			if (!hasHitSomething && player.altFunctionUse != 2)
			{
				hasHitSomething = true;

				Rectangle hitbox = GetHitbox(player);

				SpookyPlayer.ScreenShakeAmount = 8;

				SoundEngine.PlaySound(SoundID.Item62, player.itemLocation);

				for (int numDusts = 0; numDusts < 30; numDusts++)
				{
					int dustGore = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y + 10), hitbox.Width / 2, hitbox.Height / 2, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 1.5f);
					Main.dust[dustGore].color = Color.Yellow;
					Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-4f, 4f);
					Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-0.2f, 0.2f);
					Main.dust[dustGore].scale = 0.25f; 
					Main.dust[dustGore].noGravity = true;
				}
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockBack, bool crit)
		{
			if (!hasHitSomething)
			{
				hasHitSomething = true;

				SoundEngine.PlaySound(SoundID.Item62, target.Center);

				SpookyPlayer.ScreenShakeAmount = 8;

				Rectangle hitbox = GetHitbox(player);

				SoundEngine.PlaySound(SoundID.Item62, player.itemLocation);

				for (int numDusts = 0; numDusts < 30; numDusts++)
				{
					int dustGore = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width / 2, hitbox.Height / 2, ModContent.DustType<GlowyDust>(), 0f, -2f, 0, default, 1.5f);
					Main.dust[dustGore].color = Color.Yellow;
					Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
					Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-1f, 1f);
					Main.dust[dustGore].scale = 0.25f; 
					Main.dust[dustGore].noGravity = true;
				}

				Projectile.NewProjectile(Item.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<BigBoneHammerHit>(), Item.damage * 5, 0f, Main.myPlayer, 0, 0);
			}
		}
	}
}
