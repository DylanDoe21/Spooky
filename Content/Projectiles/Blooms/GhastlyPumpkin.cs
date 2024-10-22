using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Terraria.Audio;

namespace Spooky.Content.Projectiles.Blooms
{
    public class GhastlyPumpkin : ModProjectile
    {
        private static Asset<Texture2D> GlowTexture;

		public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 36;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 420;
            Projectile.penetrate = 1;
        }

		public override void PostDraw(Color lightColor)
		{
			GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Blooms/GhastlyPumpkinGlow");

			Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

            for (int i = 0; i < 4; i++)
            {
                int XOffset = Main.rand.Next(-1, 2);
                int YOffset = Main.rand.Next(-1, 2);
                
                Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(XOffset, YOffset);

			    Main.spriteBatch.Draw(GlowTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, effects, 0f);
            }
        }

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = player.direction;
            Projectile.rotation = Projectile.velocity.X * 0.04f;

            if (player.dead || !player.active || !player.GetModPlayer<BloomBuffsPlayer>().FallSoulPumpkin)
            {
                Projectile.Kill();
            }

            if (Projectile.timeLeft <= 125)
            {
                Projectile.alpha += 2;
            }

            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath6 with { Volume = 0.5f }, Projectile.Center);

                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int GhostDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, 0f, 0f, 100, default, 1.2f);
                    Main.dust[GhostDust].velocity *= 1.2f;
                    Main.dust[GhostDust].noGravity = true;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[GhostDust].scale = 0.5f;
                        Main.dust[GhostDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                Projectile.ai[0]++;
            }

			int foundTarget = HomeOnTarget();
            if (foundTarget != -1)
            {
                Projectile.spriteDirection = Projectile.direction;

				NPC target = Main.npc[foundTarget];
				Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 7;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
			}
            else
            {
				GoAbovePlayer(player);
			}
        }

        private int HomeOnTarget()
        {
            const float homingMaximumRangeInPixels = 230;

            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (target.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(target.Center);
                    if (distance <= homingMaximumRangeInPixels && (selectedTarget == -1 || Projectile.Distance(Main.npc[selectedTarget].Center) > distance))
                    {
                        selectedTarget = i;
                    }
                }
            }

            return selectedTarget;
        }

        public void GoAbovePlayer(Player player)
        {
            float goToX = player.Center.X - Projectile.Center.X + (player.direction == 1 ? -5 : 5);
            float goToY = player.Center.Y - Projectile.Center.Y - 100;

            float speed = 0.08f;
            
            if (Vector2.Distance(Projectile.Center, player.Center) >= 140)
            {
                speed = 0.12f;
            }
            else
            {
                speed = 0.08f;
            }
            
            if (Projectile.velocity.X > speed)
            {
                Projectile.velocity.X *= 0.98f;
            }
            if (Projectile.velocity.Y > speed)
            {
                Projectile.velocity.Y *= 0.98f;
            }

            if (Projectile.velocity.X < goToX)
            {
                Projectile.velocity.X = Projectile.velocity.X + speed;
                if (Projectile.velocity.X < 0f && goToX > 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X + speed;
                }
            }
            else if (Projectile.velocity.X > goToX)
            {
                Projectile.velocity.X = Projectile.velocity.X - speed;
                if (Projectile.velocity.X > 0f && goToX < 0f)
                {
                    Projectile.velocity.X = Projectile.velocity.X - speed;
                }
            }
            if (Projectile.velocity.Y < goToY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + speed;
                if (Projectile.velocity.Y < 0f && goToY > 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + speed;
                    return;
                }
            }
            else if (Projectile.velocity.Y > goToY)
            {
                Projectile.velocity.Y = Projectile.velocity.Y - speed;
                if (Projectile.velocity.Y > 0f && goToY < 0f)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y - speed;
                    return;
                }
            }
        }
    }
}