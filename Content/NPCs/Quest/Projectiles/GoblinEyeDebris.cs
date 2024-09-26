using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Quest.Projectiles
{ 
    public class GoblinEyeDebris : ModProjectile
    {
        int Bounces = 0;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
			Projectile.width = 38;
            Projectile.height = 40;
			Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 240;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Brown);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            SoundEngine.PlaySound(SoundID.GlommerBounce, Projectile.Center);

			Bounces++;
			if (Bounces >= 2)
			{
				Projectile.Kill();
			}
			else
			{
				if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.position.X = Projectile.position.X + Projectile.velocity.X;
                    Projectile.velocity.X = -oldVelocity.X * 0.8f;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.position.Y = Projectile.position.Y + Projectile.velocity.Y;
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                }
			}

			return false;
		}
		
		public override void AI()
        {
			Projectile.rotation += 0.12f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 20)
            {
                Projectile.tileCollide = false;
            }
            else
            {
                Projectile.tileCollide = true;
            }
		}

		public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.GlommerBounce, Projectile.Center);
        
            if (Projectile.frame == 3)
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, (Projectile.velocity * 0.5f) + new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MonsterEyeChunkFaded").Type);
                    }
                }
            }
            else
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, (Projectile.velocity * 0.5f) + new Vector2(Main.rand.Next(-3, 3), Main.rand.Next(-3, -1)), ModContent.Find<ModGore>("Spooky/MonsterEyeChunk").Type);
                    }
                }
            }
		}
    }
}
     
          






