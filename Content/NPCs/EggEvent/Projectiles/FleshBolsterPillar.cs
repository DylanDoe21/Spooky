using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{   
    public class FleshBolsterPillar : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> GlowTexture;
		private static Asset<Texture2D> OutlineTexture;

        public override void SetDefaults()
		{
			DrawOffsetX = 0;
			DrawOriginOffsetY = -16;
			DrawOriginOffsetX = -80;
			Projectile.width = 2;
			Projectile.height = 2;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.hide = true;
            Projectile.penetrate = -1;
			Projectile.timeLeft = 800;
		}

		public override bool? CanDamage()
		{
			return Projectile.ai[1] > 0 && Projectile.localAI[1] > 80 ? null : false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 unit = new Vector2(1, 0).RotatedBy(Projectile.rotation);
			float Distance = Projectile.ai[1];

			float point = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + unit * Distance, 8, ref point);
		}

		public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");
			OutlineTexture ??= ModContent.Request<Texture2D>(Texture + "Outline");

			lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16 - 30));

			if (Projectile.ai[1] > 0)
            {
		    	Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - new Vector2(0, 25).RotatedBy(Projectile.rotation) - Main.screenPosition,
                new Rectangle(752 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 74), lightColor, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - new Vector2(0, 25).RotatedBy(Projectile.rotation) - Main.screenPosition, 
                new Rectangle(752 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 74), Color.White, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);

				Main.EntitySpriteDraw(OutlineTexture.Value, Projectile.Center - new Vector2(0, 25).RotatedBy(Projectile.rotation) - Main.screenPosition, 
                new Rectangle(752 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 74), Color.Red * 0.65f, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);
            }

			return false;
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCsAndTiles.Add(index);
		}

		public override void AI()
		{
			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = 1;
				Projectile.rotation = Projectile.ai[0];
				Projectile.ai[0] = 0;
			}

			if (Projectile.ai[0] < 60)
			{
				Projectile.localAI[1]++;
				if (Projectile.localAI[1] <= 45)
				{
					Projectile.ai[1] += 24f;
				}

				if (Projectile.localAI[1] > 45 && Projectile.localAI[1] <= 80)
                {
					Projectile.ai[1] += Main.rand.NextFloat(-5f, 5f);
				}

				if (Projectile.localAI[1] > 80)
				{
					Projectile.ai[1] += 50;
				}

				if (Projectile.ai[1] > 520)
				{
					Projectile.ai[1] = 520;
					Projectile.ai[0]++;
				}
			}
			else
			{
				Projectile.ai[1] -= 75;

				if (Projectile.ai[1] <= 0)
				{
					Projectile.Kill();
				}
			}
		}
	}
}