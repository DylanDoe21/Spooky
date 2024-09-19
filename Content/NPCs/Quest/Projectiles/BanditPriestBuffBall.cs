using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class BanditPriestBuffBall : ModProjectile
    {
        int RandomFrameStart = Main.rand.Next(0, 6);

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> IconTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(Projectile.width * 0.5f, Projectile.height * 0.5f);

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity;
            Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Cyan);

                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center + circular - Main.screenPosition, rectangle, color, Projectile.rotation, drawOrigin, 1.1f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, rectangle, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);

			return false;
		}

        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[0] > 10)
            {
                IconTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/Projectiles/BanditPriestBuffBallIcons");

                int height = IconTexture.Height() / Main.projFrames[Projectile.type];
                int frameHeight = height * Projectile.frame;
                Rectangle rectangle = new Rectangle(0, frameHeight, IconTexture.Width(), height);

                Main.EntitySpriteDraw(IconTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                rectangle, Color.White, Projectile.rotation, new Vector2(IconTexture.Width() / 2f, height / 2f), Projectile.scale, SpriteEffects.None, 0);
            }
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Player Target = Main.player[Projectile.owner];

            Projectile.timeLeft = 5;

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 10)
            {
                Projectile.frame = RandomFrameStart;
            }

            if (Projectile.ai[0] <= 100)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 2)
                {
                    Projectile.frame++;
                    Projectile.frameCounter = 0;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }

                Projectile.velocity *= 0.98f;
            }

            if (Projectile.ai[0] >= 100 && Projectile.ai[0] < 160)
            {
                Projectile.velocity *= 0;
            }

            //go to the player and inflict the debuff on them
            if (Projectile.ai[0] >= 160)
            {
                Vector2 desiredVelocity = Projectile.DirectionTo(Target.Center) * 45;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);

                if (Projectile.Hitbox.Intersects(Target.Hitbox))
                {
                    switch (Projectile.frame)
                    {
                        case 0:
                        {
                            Target.AddBuff(BuffID.PotionSickness, 800);
                            break;
                        }
                        case 1:
                        {
                            Target.AddBuff(BuffID.Slow, 600);
                            break;
                        }
                        case 2:
                        {
                            Target.AddBuff(BuffID.Weak, 700);
                            break;
                        }
                        case 3:
                        {
                            Target.AddBuff(BuffID.BrokenArmor, 1200);
                            break;
                        }
                        case 4:
                        {
                            Target.AddBuff(BuffID.Cursed, 420);
                            break;
                        }
                        case 5:
                        {
                            Target.AddBuff(BuffID.Confused, 480);
                            break;
                        }
                    }

                    Projectile.Kill();
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

            float maxAmount = 30;
			int currentAmount = 0;
			while (currentAmount <= maxAmount)
			{
				Vector2 velocity = new Vector2(5f, 5f);
				Vector2 Bounds = new Vector2(3f, 3f);
				float intensity = 5f;

				Vector2 vector12 = Vector2.UnitX * 0f;
				vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
				vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
				int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.XenonMoss, 0f, 0f, 100, default, 2f);
				Main.dust[num104].noGravity = true;
				Main.dust[num104].position = Projectile.Center + vector12;
				Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
				currentAmount++;
			}
        }
    }
}