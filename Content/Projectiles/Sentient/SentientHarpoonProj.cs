using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Enums;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientHarpoonProj : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        //handle
        public int HandleHeight => 56;
        //segment 1
        public int BodyType1StartY => 58;
        public int BodyType1SectionHeight => 12;
        //segment 2
        public int BodyType2StartY => 72;
        public int BodyType2SectionHeight => 12;
        //tip segment
        public int TipStartY => 86;
        public int TipHeight => 14;

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            // If the velocity is zero, don't draw anything.
            // Doing so would lead to various divison by 0 errors during the normalization process.
            if (Projectile.velocity == Vector2.Zero)
            {
                return false;
            }

            Vector2 normalizedVelocity = Vector2.Normalize(Projectile.velocity);

            float speed = Projectile.velocity.Length() + 16f - 40f * Projectile.scale;

            var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Vector2 bodyDrawPosition = Projectile.Center.Floor() + normalizedVelocity * Projectile.scale * 20f;

            DrawSegments(speed, normalizedVelocity, lightColor, bodyDrawPosition, effects);

            DrawHandleSprite(lightColor, effects);

            return false;
        }

        public void DrawHandleSprite(Color color, SpriteEffects effects)
        {
            Rectangle handleFrame = new Rectangle(0, 0, ProjTexture.Width(), HandleHeight);
            Main.EntitySpriteDraw(ProjTexture.Value,
            Projectile.Center.Floor() - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
            new Rectangle?(handleFrame),
            color,
            Projectile.rotation + MathHelper.Pi,
            handleFrame.Size() / 2f - Vector2.UnitY * 4f,
            Projectile.scale,
            effects,
            0);
        }

        public void DrawSegments(float speed, Vector2 normalizedVelocity, Color lightColor, Vector2 bodyDrawPosition, SpriteEffects effects)
        {
            Rectangle BodyFrame = new Rectangle(0, BodyType1StartY, ProjTexture.Width(), BodyType1SectionHeight);
            //bool reducedType1BodyCount = speed < 100f;
            int numSegments = (int)(speed / 10);
            if (speed > 0f)
            {
                float speedRatio = speed / numSegments;
                bodyDrawPosition += normalizedVelocity * speedRatio * 0.25f;
                for (int i = 0; i < numSegments; i++)
                {
                    float drawPositionDeltaMult = speedRatio;
                    if (i == 0)
                    {
                        drawPositionDeltaMult *= 0.75f;
                    }

                    if (i == numSegments - 1)
                    {
                        BodyFrame = new Rectangle(0, TipStartY, ProjTexture.Width(), TipHeight);
                    }
                    else
                    {
                        if (i % 2 == 0)
                        {
                            BodyFrame = new Rectangle(0, BodyType2StartY, ProjTexture.Width(), BodyType2SectionHeight);
                        }
                        else
                        {
                            BodyFrame = new Rectangle(0, BodyType1StartY, ProjTexture.Width(), BodyType1SectionHeight);
                        }
                    }

                    Main.EntitySpriteDraw(ProjTexture.Value,
                    bodyDrawPosition - Main.screenPosition + Vector2.UnitY * Main.player[Projectile.owner].gfxOffY,
                    new Rectangle?(BodyFrame),
                    lightColor,
                    Projectile.rotation + MathHelper.Pi,
                    new Vector2(BodyFrame.Width / 2, 0f),
                    Projectile.scale,
                    effects,
                    0);
                    bodyDrawPosition += normalizedVelocity * drawPositionDeltaMult;
                }
            }
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + unit, Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = player.direction;

            if (Projectile.localAI[1] > 0f)
            {
                Projectile.localAI[1] -= 1f;
            }

            Projectile.alpha -= 42;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = Projectile.velocity.ToRotation();
            }

            float direction = (Projectile.localAI[0].ToRotationVector2().X >= 0f).ToDirectionInt();
            if (Projectile.ai[1] <= 0f)
            {
                direction *= -1f;
            }

            Vector2 velocityAdditive = (direction * (Projectile.ai[0] / 30f * MathHelper.TwoPi - MathHelper.PiOver2)).ToRotationVector2();

            velocityAdditive.Y *= (float)Math.Sin(Projectile.ai[1]);
            if (Projectile.ai[1] <= 0f)
            {
                velocityAdditive.Y *= -1f;
            }

            velocityAdditive = velocityAdditive.RotatedBy(Projectile.localAI[0]);
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] < 30f)
            {
                Projectile.velocity += 32f * velocityAdditive;
            }
            else
            {
                Projectile.Kill();
            }
            
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            Vector2 centerDelta = Main.OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction != 1)
            {
                centerDelta.X = player.bodyFrame.Width - centerDelta.X;
            }
            if (player.gravDir != 1f)
            {
                centerDelta.Y = player.bodyFrame.Height - centerDelta.Y;
            }
            if (player.heldProj == -1)
            {
                player.heldProj = Projectile.whoAmI;
            }

            centerDelta -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
            Projectile.Center = player.RotatedRelativePoint(player.position + centerDelta, true) - Projectile.velocity;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            target.AddBuff(ModContent.BuffType<SentientHarpoonSlow>(), 180);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }

            float zero = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity, 16f * Projectile.scale, ref zero))
            {
                return true;
            }

            return false;
        }
    }
}