using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
	public class SentientWeatherPainProj : ModProjectile
	{
        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> ChickenTexture;

        public static readonly SoundStyle UseSound = new("Spooky/Content/Sounds/SentientChickenScream", SoundType.Sound) { Volume = 0.12f };

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
            Projectile.width = 54;
            Projectile.height = 136;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
		}

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            Player player = Main.player[Projectile.owner];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);
			ChickenTexture ??= ModContent.Request<Texture2D>(Texture + "Chicken");

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
			
			Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            bool Shake = Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 2 && Projectile.localAI[0] < ItemGlobal.ActiveItem(player).useTime;
			Main.EntitySpriteDraw(ChickenTexture.Value, vector + (Shake ? Main.rand.NextVector2Square(-5, 5) : Vector2.Zero), rectangle, 
            Projectile.GetAlpha(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
			
            return false;
		}

        public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead || player.noItems || player.CCed) 
            {
                Projectile.Kill();
            }

            if (!player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
			{
				Projectile.Kill();
			}

			if (Projectile.owner == Main.myPlayer)
			{
				Vector2 ProjDirection = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
				ProjDirection.Normalize();
				Projectile.ai[0] = ProjDirection.X;
				Projectile.ai[1] = ProjDirection.Y;
			}

			Vector2 direction = new Vector2(Projectile.ai[0], Projectile.ai[1]);

			Projectile.direction = Projectile.spriteDirection = direction.X > 0 ? 1 : -1;

			if (Projectile.direction >= 0)
			{
				Projectile.rotation = direction.ToRotation() - 1.57f * (float)Projectile.direction;
			}
			else
			{
				Projectile.rotation = direction.ToRotation() + 1.57f * (float)Projectile.direction;
			}

			player.itemRotation = Projectile.rotation;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, player.itemRotation);

            Projectile.position = new Vector2(player.MountedCenter.X - Projectile.width / 2, player.MountedCenter.Y - Projectile.height / 2);

            Projectile.timeLeft = 5;

            Projectile.localAI[0]++;

            if (Projectile.localAI[0] < ItemGlobal.ActiveItem(player).useTime / 2)
            {
                Projectile.timeLeft = 60;
                Projectile.frame = 0;
            }

            if (Projectile.localAI[0] == ItemGlobal.ActiveItem(player).useTime / 2)
            {
                SoundEngine.PlaySound(UseSound, Projectile.Center);
            }

            if (Projectile.localAI[0] >= ItemGlobal.ActiveItem(player).useTime / 2 && Projectile.localAI[0] < ItemGlobal.ActiveItem(player).useTime)
            {
                Projectile.frame = 1;
            }

            if (Projectile.localAI[0] == ItemGlobal.ActiveItem(player).useTime)
            {
				SoundEngine.PlaySound(SoundID.DD2_BookStaffCast with { Pitch = -0.5f }, Projectile.Center);

                if (Projectile.localAI[1] > 0 && player.CheckMana(ItemGlobal.ActiveItem(player), ItemGlobal.ActiveItem(player).mana, false, false))
                {
                    player.statMana -= ItemGlobal.ActiveItem(player).mana;
                }

                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 ShootSpeed = Main.MouseWorld - new Vector2(Projectile.Center.X, Projectile.Center.Y);
                    ShootSpeed.Normalize();
                    ShootSpeed *= 25;

                    Vector2 muzzleOffset = Vector2.Normalize(new Vector2(ShootSpeed.X, ShootSpeed.Y)) * 40f;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + muzzleOffset, ShootSpeed, 
                    ModContent.ProjectileType<SentientWeatherPainTornado>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai1: Main.rand.Next(0, 4));
                }

                if (Projectile.localAI[1] <= 0)
                {
                    Projectile.localAI[1]++;
                }
            }

            if (Projectile.localAI[0] > ItemGlobal.ActiveItem(player).useTime + 10)
            {
                if (player.channel)
                {
                    Projectile.localAI[0] = 0;
                }
                else
                {
                    Projectile.Kill();
                }
            }

			if (direction.X > 0)
			{
				player.direction = 1;
			}
			else
			{
				player.direction = -1;
			}

			player.heldProj = Projectile.whoAmI;
            player.SetDummyItemTime(2);
        }
	}
}