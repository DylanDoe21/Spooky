using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{ 
    public class SolarThorn : ModProjectile
    {
		public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBoneMagic2", SoundType.Sound);

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Solar Thorn");
		}

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
			Projectile.timeLeft = 600;
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
			lightColor = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			if (Projectile.ai[1] > 0)
            {
		    	Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, 
                new Rectangle(1200 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 34), Color.OrangeRed, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);
            }

			return false;
		}

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
			behindNPCsAndTiles.Add(index);
		}

		//The AI of the projectile
		public override void AI()
		{
			Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0f);

			if (Projectile.localAI[0] == 0)
			{
				Projectile.localAI[0] = 1;
				Projectile.rotation = Projectile.ai[0];
				Projectile.ai[0] = 0;
			}

			if (Projectile.ai[0] < 30)
			{
				Projectile.localAI[1]++;

				if (Projectile.localAI[1] <= 60)
				{
					Projectile.ai[1] += Main.rand.NextFloat(15f, 25f);
				}

				if (Projectile.localAI[1] > 60 && Projectile.localAI[1] <= 100)
                {
					Projectile.ai[1] += Main.rand.NextFloat(-5f, 5f);
				}

				if (Projectile.localAI[1] > 100 && Projectile.localAI[1] <= 130)
				{
					Projectile.ai[1] -= 7;
				}

				if (Projectile.localAI[1] == 175)
				{
					SoundEngine.PlaySound(MagicCastSound, Projectile.Center);
				}

				if (Projectile.localAI[1] > 175)
				{
					Projectile.ai[1] += 70;
				}

				if (Projectile.ai[1] > 1000)
				{
					Projectile.ai[1] = 1000;

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