using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
	public class CatacombKey2Proj : ModProjectile
	{
		public override string Texture => "Spooky/Content/Items/Catacomb/Misc/CatacombKey2";

        Vector2 SaveProjectilePosition;

        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
		{
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;                   			 
            Projectile.height = 24;  
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
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Red) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
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

        public override void OnKill(int timeLeft)
        {
			for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RedTorch, 0f, 0f, 100, default, 2f);
                Main.dust[DustGore].velocity *= 5f;
                Main.dust[DustGore].noGravity = true;
            }

			string text = Language.GetTextValue("Mods.Spooky.Dialogue.CatacombKeys.Key2");

			if (Main.netMode != NetmodeID.Server)
			{
				Main.NewText(text, Color.Red);
			}
			else if (Main.netMode == NetmodeID.Server)
			{
				ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), Color.Red);
			}

			if (!Flags.CatacombKey2)
			{
				Flags.CatacombKey2 = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}
		}
    }
}