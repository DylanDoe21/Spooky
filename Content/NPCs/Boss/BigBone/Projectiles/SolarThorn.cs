using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{ 
    public class SolarThorn : ModProjectile
    {
		private static Asset<Texture2D> ProjTexture;

		public static readonly SoundStyle MagicCastSound = new("Spooky/Content/Sounds/BigBone/BigBoneMagic2", SoundType.Sound) { PitchVariance = 0.6f };

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
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			if (Projectile.ai[1] > 0)
            {
		    	Main.EntitySpriteDraw(ProjTexture.Value, Projectile.Center - Main.screenPosition, 
                new Rectangle(1200 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 20), Color.White, 
                Projectile.rotation, new Vector2(17, 17), 1f, SpriteEffects.None, 0);
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
					Projectile.ai[1] += 20f;
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