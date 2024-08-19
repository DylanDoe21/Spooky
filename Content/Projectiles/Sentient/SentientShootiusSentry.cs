using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientShootiusSentry : ModProjectile
    {
        public bool isAttacking = false;

        private static Asset<Texture2D> AuraTexture;
        private static Asset<Texture2D> PupilTexture;
        private static Asset<Texture2D> PupilLargeTexture;

        public override void SetStaticDefaults()
		{
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 60;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (isAttacking)
            {
                AuraTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientShootiusSentryEyeDraw");

                Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.DeepPink);

                Vector2 drawOrigin = new(AuraTexture.Width() * 0.5f, Projectile.height * 0.5f);

                for (int numEffect = 0; numEffect < 3; numEffect++)
                {
                    Color newColor = color;
                    newColor = Projectile.GetAlpha(newColor);
                    newColor *= 1f;
                    Vector2 vector = new Vector2(Projectile.Center.X - 2, Projectile.Center.Y + 3) + (numEffect / 3 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                    Rectangle rectangle = new(0, AuraTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, AuraTexture.Width(), AuraTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(AuraTexture.Value, vector, rectangle, newColor, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            PupilTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientShootiusSentryPupil");
            PupilLargeTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Sentient/SentientShootiusSentryPupilWide");

            Vector2 drawOrigin = new(PupilTexture.Width() * 0.5f, Projectile.height * 0.5f);
            Rectangle rectangle = new(0, PupilTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, PupilTexture.Width(), PupilTexture.Height() / Main.projFrames[Projectile.type]);
            Main.EntitySpriteDraw(isAttacking ? PupilLargeTexture.Value : PupilTexture.Value, Projectile.Center - Main.screenPosition, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

        public override void AI()
        {
            //animation
            if (!isAttacking)
            {
                //reset ai timer
                Projectile.ai[0] = 0;

                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 5)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                Projectile.frame = 0;
            }

            //fall down constantly
            Projectile.velocity.Y += 0.65f;
            if (Projectile.velocity.Y > 20f)
            {
                Projectile.velocity.Y = 20f;
            }

            //target an enemy
            for (int i = 0; i < 200; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 500f)
                {
					Shoot(Target);

					break;
				}
                else
                {
                    isAttacking = false;
                }

				NPC NPC = Main.npc[i];
                if (NPC.active && Target.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 500f)
                {
					Shoot(NPC);

					break;
				}
                else
                {
                    isAttacking = false;
                }
            }
        }

        public void Shoot(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;

            //spawn dusts
            if (Projectile.ai[0] <= 50)
            {
                int MaxDusts = Main.rand.Next(5, 15);
                for (int numDusts = 0; numDusts < MaxDusts; numDusts++)
                {
                    Vector2 dustPos = (Vector2.One * new Vector2((float)Projectile.width / 2f, (float)Projectile.height / 2f) * 0.5f).RotatedBy((double)((float)(numDusts - (MaxDusts / 2 - 1)) * 6.28318548f / (float)MaxDusts), default(Vector2)) + new Vector2(Projectile.Center.X + 2, Projectile.Center.Y - 23);
                    Vector2 velocity = dustPos - new Vector2(Projectile.Center.X + 2, Projectile.Center.Y - 23);
                    
                    int dustEffect = Dust.NewDust(dustPos + velocity, 0, 0, ModContent.DustType<GlowyDust>(), velocity.X * 2f, velocity.Y * 2f, 100, default, 1f);
                    Main.dust[dustEffect].color = Color.Red;
                    Main.dust[dustEffect].scale = 0.075f;
                    Main.dust[dustEffect].noGravity = true;
                    Main.dust[dustEffect].noLight = false;
                    Main.dust[dustEffect].velocity = Vector2.Normalize(velocity) * Main.rand.NextFloat(-1f, -0.5f);
                    Main.dust[dustEffect].fadeIn = 1.3f;
                }
            }

			if (Projectile.ai[0] == 50 || Projectile.ai[0] == 60 || Projectile.ai[0] == 70)
			{
				SoundEngine.PlaySound(SoundID.Item45, Projectile.Center);

				float Speed = 30f;
				Vector2 vector = new(Projectile.position.X + 2 + (Projectile.width / 2), Projectile.position.Y - 23 + (Projectile.height / 2));
				float rotation = (float)Math.Atan2(vector.Y - (target.position.Y + (target.height * 0.5f)), vector.X - (target.position.X + (target.width * 0.5f)));
				Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X + 7, Projectile.Center.Y - 21,
				perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<SentientShootiusBolt>(), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);
            }

            if (Projectile.ai[0] >= 80)
            {
                Projectile.ai[0] = 0;
            }
		}
    }
}