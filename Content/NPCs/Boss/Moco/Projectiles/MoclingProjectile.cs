using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
	public class MoclingProjectile : ModProjectile
	{
        public override string Texture => "Spooky/Content/NPCs/SpookyHell/Mocling";

        Vector2 SaveProjPosition;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 26;
			Projectile.friendly = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 240;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.DarkGreen) * ((float)(Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }
            
            return true;
        }

		public override void AI()
		{
            Player player = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                Vector2 GoTo = player.Center + new Vector2(Main.rand.Next(-50, 51), Main.rand.Next(-50, -100));

                if (Projectile.Distance(GoTo) >= 200f)
                { 
                    GoTo -= Projectile.DirectionTo(GoTo) * 100f;
                }

                Vector2 GoToVelocity = GoTo - Projectile.Center;

                float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

                float velocityLength = GoToVelocity.Length();

                if (velocityLength > 18f)
                { 
                    velocityLength = 18f;
                }

                Projectile.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            }

            if (Projectile.ai[0] > 1 && Projectile.ai[0] < 30)
            {
                Projectile.velocity *= 0.92f;
            }
            
            if (Projectile.ai[0] == 45)
            {
                SaveProjPosition = Projectile.Center;
            }

            if (Projectile.ai[0] > 45 && Projectile.ai[0] <= 60)
            {
                Projectile.Center = new Vector2(SaveProjPosition.X, SaveProjPosition.Y);
                Projectile.Center += Main.rand.NextVector2Square(-3, 3);
            }

            if (Projectile.localAI[0] == 65)
            {
                double Velocity = Math.Atan2(player.position.Y - Projectile.position.Y, player.position.X - Projectile.position.X);
                Projectile.velocity = new Vector2((float)Math.Cos(Velocity), (float)Math.Sin(Velocity)) * 12;
            }

            if (Projectile.localAI[0] > 70)
            {
                Projectile.tileCollide = true;
            }
		}

        public override void OnKill(int timeLeft)
		{
        	for (int numDusts = 0; numDusts < 25; numDusts++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                
				if (Main.dust[newDust].position != Projectile.Center)
				{
					Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
				}
			}
		}
	}
}