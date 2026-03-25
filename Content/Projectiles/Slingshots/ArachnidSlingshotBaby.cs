using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Slingshots
{
    public class ArachnidSlingshotBaby : ModProjectile
    {
        bool IsStickingToTarget = false;

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[8];

        private static Asset<Texture2D> TrailTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileGlobal.IsSlingshotAmmoProj[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
			Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (runOnce)
			{
				return false;
			}

			TrailTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailCircle");

			Vector2 drawOrigin = new(TrailTexture.Width() * 0.5f, TrailTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				float scale = Projectile.scale * (trailLength.Length - k) / (float)trailLength.Length;
				scale *= 1f;

                Color color = Projectile.GetAlpha(Color.White);

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(TrailTexture.Value, drawPos, null, color * 0.5f, Projectile.rotation, drawOrigin, scale * 0.35f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 300);

            if (!IsStickingToTarget)
            {
                Projectile.timeLeft = 60;
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            if (IsStickingToTarget) 
            {
                Projectile.frame = 3;

				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int npcTarget = (int)Projectile.ai[1];
                if (npcTarget < 0 || npcTarget >= 200) 
                {
                    Projectile.Kill();
                }
                else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage) 
                {
                    Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
                    Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
                }
                else 
                {
                    Projectile.Kill();
                }
			}
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 3)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 2)
                    {
                        Projectile.frame = 0;
                    }
                }

                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			    Projectile.rotation += 0f * (float)Projectile.direction;

                if (runOnce)
                {
                    for (int i = 0; i < trailLength.Length; i++)
                    {
                        trailLength[i] = Vector2.Zero;
                    }

                    runOnce = false;
                }

                Vector2 current = Projectile.Center;
                for (int i = 0; i < trailLength.Length; i++)
                {
                    Vector2 previousPosition = trailLength[i];
                    trailLength[i] = current;
                    current = previousPosition;
                }

                if (IsColliding())
                {
                    Projectile.ai[0] = 1;
                }
                if (Projectile.ai[0] > 0)
                {
                    Projectile.velocity *= 0.95f;

                    Projectile.alpha += 5;
                    if (Projectile.alpha >= 255)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        public bool IsColliding()
        {
            int minTilePosX = (int)(Projectile.position.X / 16) - 1;
            int maxTilePosX = (int)((Projectile.position.X + Projectile.width) / 16) + 2;
            int minTilePosY = (int)(Projectile.position.Y / 16) - 1;
            int maxTilePosY = (int)((Projectile.position.Y + Projectile.height) / 16) + 2;
            if (minTilePosX < 0)
            {
                minTilePosX = 0;
            }
            if (maxTilePosX > Main.maxTilesX)
            {
                maxTilePosX = Main.maxTilesX;
            }
            if (minTilePosY < 0)
            {
                minTilePosY = 0;
            }
            if (maxTilePosY > Main.maxTilesY)
            {
                maxTilePosY = Main.maxTilesY;
            }

            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    if (Main.tile[i, j] != null && WorldGen.SolidTile(i, j))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);

                        if (Projectile.position.X + Projectile.width > vector2.X && Projectile.position.X < vector2.X + 16.0 && 
                        (Projectile.position.Y + Projectile.height > (double)vector2.Y && Projectile.position.Y < vector2.Y + 16.0))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}