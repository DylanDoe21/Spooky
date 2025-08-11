using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome.Projectiles
{   
    public class ZomboidRootThornFire : ModProjectile
    {
        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ProjTexture;

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
			Projectile.timeLeft = 240;
		}

		public override bool? CanDamage()
		{
			return Projectile.ai[1] > 0 ? null : false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			Vector2 unit = new Vector2(1, 0).RotatedBy(Projectile.rotation);
			float Distance = Projectile.ai[1];

			float point = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + unit * Distance, 5, ref point);
		}

		public override bool PreDraw(ref Color lightColor)
		{
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            GlowTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/SpookyBiome/Projectiles/ZomboidRootThornFireGlow");

            lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));

            if (Projectile.ai[1] > 0)
            {
		    	Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, 
                new Rectangle(60 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 14), lightColor, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);

				for (int i = 0; i < 3; i++)
				{
					int XOffset = Main.rand.Next(-1, 2);
					int YOffset = Main.rand.Next(-1, 2);

					Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(XOffset, YOffset), 
					new Rectangle(60 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 14), Color.White * 0.5f, 
					Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);
				}
            }

			return false;
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCsAndTiles.Add(index);
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 300);
        }

		public override void AI()
		{
			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = 1;
				Projectile.rotation = Projectile.ai[0];
				Projectile.ai[0] = 0;
			}

			if (Projectile.ai[0] < 75)
			{
				Projectile.ai[1] += 10;

				if (Projectile.ai[1] == 10)
				{
					for (int numProjectiles = 0; numProjectiles < 3; numProjectiles++)
					{
						if (Main.netMode != NetmodeID.MultiplayerClient)
                		{
							int GreekFire = Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, 
							Main.rand.Next(-2, 3), Main.rand.Next(-6, -2), ProjectileID.GreekFire3, Projectile.damage, 0f, Main.myPlayer, 0, 0);
							Main.projectile[GreekFire].timeLeft = 120;
						}
					}
				}

				if (Projectile.ai[1] > 60)
				{
					Projectile.ai[1] = 60;

					Projectile.ai[0]++;
				}
			}
			else
			{
				Projectile.ai[1] -= 10;
			}
		}
	}
}