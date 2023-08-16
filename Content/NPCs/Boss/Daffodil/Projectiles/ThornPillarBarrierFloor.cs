using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.Daffodil.Projectiles
{ 
    public class ThornPillarBarrierFloor : ModProjectile
	{
		public static readonly SoundStyle ThornSpawnSound = new("Spooky/Content/Sounds/Daffodil/ThornBarrier", SoundType.Sound);

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

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.velocity.Y = -10;
        }

        public override bool PreDraw(ref Color lightColor)
		{
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;

			if (Projectile.ai[1] > 0)
            {
				float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f;

		    	Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, 
                new Rectangle(695 - (int)Projectile.ai[1], Projectile.frame, (int)Projectile.ai[1] + 17, 42), Color.White, 
                Projectile.rotation, new Vector2(17, 17), Projectile.scale + fade / 9, SpriteEffects.None, 0);
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
			Lighting.AddLight(Projectile.Center, 0.4f, 0f, 0f);

			if (Projectile.localAI[0] == 0)
			{
				SoundEngine.PlaySound(ThornSpawnSound, Projectile.Center);

				Projectile.localAI[0] = 1;
				Projectile.rotation = Projectile.ai[0];
				Projectile.ai[0] = 0;
			}

			if (Projectile.ai[0] < 60)
			{
				Projectile.ai[1] += 35;

				if (Projectile.ai[1] > 695)
				{
					Projectile.ai[1] = 695;

					Projectile.ai[0]++;
				}
			}
			else
			{
				//do not retract unless daffodil is not active
				if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()))
				{
					Projectile.ai[1] -= 35;
				}
				else
				{
					Projectile.timeLeft = 60;
				}

				if (Projectile.ai[1] <= 0)
				{
					Projectile.Kill();
				}
			}
		}
	}
}