using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class CatacombKey1Proj : ModProjectile
	{
        Vector2 SaveProjectilePosition;

		public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;                   			 
            Projectile.height = 32;  
            Projectile.friendly = true;       
			Projectile.hostile = false;                                 			  		
            Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
		}

		public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = new(tex.Width * 0.5f, Projectile.height * 0.5f);

			for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
				float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Yellow) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (tex.Height / Main.projFrames[Projectile.type]) * Projectile.frame, tex.Width, tex.Height / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(tex, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            }

            return true;
        }

		public override bool? CanCutTiles()
        {
            return false;
        }

		public override void AI()
		{
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 0 && Projectile.ai[0] < 25)
			{
				if (Projectile.alpha > 0)
				{
					Projectile.alpha -= 10;
				}

                Projectile.velocity *= 1.1f;
            }

			if (Projectile.ai[0] == 25)
			{
				Projectile.velocity *= 0;
			}

			if (Projectile.ai[0] == 60)
			{
				SaveProjectilePosition = Projectile.Center;
			}

			if (Projectile.ai[0] > 60 && Projectile.ai[0] < 120)
			{
				Projectile.Center = new Vector2(SaveProjectilePosition.X, SaveProjectilePosition.Y);
				Projectile.Center += Main.rand.NextVector2Square(-4, 4);
			}

			if (Projectile.ai[0] >= 120)
            {
				SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);

				Projectile.Kill();
			}
		}

        public override void Kill(int timeLeft)
        {
			for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.YellowTorch, 0f, 0f, 100, default, 2f);
                Main.dust[DustGore].velocity *= 5f;
                Main.dust[DustGore].noGravity = false;
            }

			string text = "The yellow barrier has been opened!";

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(text, Color.Yellow);
			}
			else
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), Color.Yellow);
			}

			if (!Flags.CatacombKey1)
			{
				Flags.CatacombKey1 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}

			if (Main.netMode != NetmodeID.Server)
			{
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0, 0), ModContent.Find<ModGore>("Spooky/KeyYellowGore1").Type);
				Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, new Vector2(0, 0), ModContent.Find<ModGore>("Spooky/KeyYellowGore2").Type);
			}
		}
    }
}