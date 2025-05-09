using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class PandoraCuffProj : ModProjectile
    {
        public bool IsStickingToTarget = false;

        private static Asset<Texture2D> ChainTexture;

        public override void SetDefaults()
        {
			Projectile.width = 26;
            Projectile.height = 22;     
			Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Catacomb/PandoraCuffProjChain");

            Vector2 ParentCenter = Main.player[Projectile.owner].Center;

            Rectangle? chainSourceRectangle = null;
            float chainHeightAdjustment = 0f;

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Projectile.Center;
            Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10;
            }

            float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
            int chainCount = 0;
            float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

            while (chainLengthRemainingToDraw > 0f)
            {
                Color chainDrawColor = Color.Cyan * 0.5f;

                Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                chainDrawPosition += unitVectorToParent * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            return true;
        }

        public override bool? CanDamage()
        {
			return false;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                Projectile.Kill();
            }

            NPC target = Main.npc[(int)Projectile.ai[0]];

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            if (!IsStickingToTarget && Projectile.Hitbox.Intersects(target.Hitbox))
            {
                Projectile.velocity *= 0;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }

            if (IsStickingToTarget) 
            {
                if (target.active && !target.dontTakeDamage) 
                {
                    Projectile.Center = target.Center;
                    target.takenDamageMultiplier = 1.5f;
                }
                else 
                {
                    Projectile.Kill();
                }
			}
			else 
            {
                Projectile.timeLeft = 300;

                Vector2 desiredVelocity = Projectile.DirectionTo(target.Center) * 70;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 20);
			}
		}

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.Unlock, Projectile.Center);

            for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Platinum, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
			}
        }
    }
}