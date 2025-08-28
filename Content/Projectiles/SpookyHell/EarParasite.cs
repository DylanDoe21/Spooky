using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
	public class EarParasite : ModProjectile
	{
		bool Shake = false;

		private static Asset<Texture2D> ChainTexture;

		public static readonly SoundStyle ScreechSound = new("Spooky/Content/Sounds/EggEvent/EarWormScreech", SoundType.Sound) { Volume = 1.35f };

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 7;
        }

		public override void SetDefaults()
		{
			Projectile.width = 36;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			if (!player.dead)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/EarWormSegment");

				bool flip = false;
				if (player.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = Projectile.Center - new Vector2(12 * (flip ? -1 : 1), 2).RotatedBy(Projectile.rotation);
				Vector2 p0 = player.Center;
				Vector2 p1 = player.Center;
				Vector2 p2 = myCenter - new Vector2(20 * (flip ? -1 : 1), 2).RotatedBy(Projectile.rotation);
				Vector2 p3 = myCenter;

				int segments = 12;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = (drawPosNext - drawPos2);
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					lightColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			return true;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
            
			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().GiantEar = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().GiantEar)
            {
				Projectile.timeLeft = 2;
            }

            Projectile.frameCounter++;
			if (Projectile.frameCounter >= 6) 
			{
				Projectile.frameCounter = 0;
				
                Projectile.frame++;
				if (Projectile.frame >= 7) 
				{
					Projectile.frame = 0;
				}
			}

			Projectile.spriteDirection = -player.direction;

			bool IsWeapon = ItemGlobal.ActiveItem(player).damage > 0 && ItemGlobal.ActiveItem(player).pick <= 0 && ItemGlobal.ActiveItem(player).hammer <= 0 && 
			ItemGlobal.ActiveItem(player).axe <= 0 && ItemGlobal.ActiveItem(player).mountType <= 0;

			if (player.controlUseItem && IsWeapon)
			{
				if (Shake)
				{
					Projectile.rotation += 0.1f;
					if (Projectile.rotation > 0.2f)
					{
						Shake = false;
					}
				}
				else
				{
					Projectile.rotation -= 0.1f;
					if (Projectile.rotation < -0.2f)
					{
						Shake = true;
					}
				}

				Projectile.ai[0]++;

				if (Projectile.ai[0] >= ItemGlobal.ActiveItem(player).useTime * 2)
				{
					SoundEngine.PlaySound(ScreechSound, Projectile.Center);

					if (Projectile.owner == Main.myPlayer)
                	{
						Vector2 ShootSpeed = Main.MouseWorld - Projectile.Center;
						ShootSpeed.Normalize();
						ShootSpeed *= 10;

						//damage only scales based off of your item if it does over 60 damage to prevent the damage from being obnoxiously low
						int Damage = ItemGlobal.ActiveItem(player).damage > 60 ? ItemGlobal.ActiveItem(player).damage : 60;

						Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, ModContent.ProjectileType<EarParasiteSoundBase>(), Damage, 0, player.whoAmI);
					}

					Projectile.ai[0] = 0;
				}
			}
			else
			{
				Projectile.rotation = 0;
			}

			Projectile.ai[1] += 0.05f;

			Vector2 offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 20f * (Projectile.ai[1] / 60f));

			Projectile.Center = new Vector2(player.MountedCenter.X, player.MountedCenter.Y - 65) + offsetFromPlayer * 4f;
			Projectile.velocity = Vector2.Zero;
		}
	}
}