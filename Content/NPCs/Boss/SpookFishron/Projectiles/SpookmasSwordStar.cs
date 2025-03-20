using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.SpookFishron.Projectiles
{
    public class SpookmasSwordStar : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

		public override void SetDefaults()
		{
			Projectile.width = 22;
            Projectile.height = 24;
			Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = true;
			Projectile.ignoreWater = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 55;
			Projectile.aiStyle = -1;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 0.8f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Gold) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

		public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f;

			//make the star scale up and down rapidly
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 2)
            {
                Projectile.scale -= 0.3f;
            }
            if (Projectile.ai[0] >= 2)
            {
                Projectile.scale += 0.3f;
            }

			if (Projectile.ai[0] > 4)
            {
                Projectile.ai[0] = 0;
                Projectile.scale = 1.1f;
            }

			Vector2 desiredVelocity = Projectile.DirectionTo(new Vector2(player.Center.X, player.Center.Y - 370)) * 22;
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 1f / 7);
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);

			for (int numProjectiles = 0; numProjectiles < 40; numProjectiles++)
			{
				int VelocityX = Main.rand.Next(-90, 91);
				int VelocityY = Main.rand.Next(-15, -8);

				int newProj = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(VelocityX, VelocityY), ProjectileID.OrnamentFriendly, Projectile.damage, Projectile.knockBack, Projectile.owner);
				Main.projectile[newProj].friendly = false;
				Main.projectile[newProj].hostile = true;
			}
		}
	}
}
     
          






