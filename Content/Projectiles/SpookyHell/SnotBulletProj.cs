using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class SnotBulletProj : ModProjectile
    {
		public override string Texture => "Spooky/Content/Projectiles/TrailSquare";

        bool runOnce = true;
		Vector2[] trailLength = new Vector2[3];

		private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
			Projectile.width = 16;                   			 
            Projectile.height = 16;     
			Projectile.friendly = true;                               			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;  
            Projectile.penetrate = 2;
			Projectile.timeLeft = 300;
            Projectile.alpha = 255;
		}

        public override bool PreDraw(ref Color lightColor)
		{
			if (runOnce)
			{
				return false;
			}

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, ProjTexture.Height() * 0.5f);
			Vector2 previousPosition = Projectile.Center;

			for (int k = 0; k < trailLength.Length; k++)
			{
				Color color = Color.Lime;

				if (trailLength[k] == Vector2.Zero)
				{
					return true;
				}

				Vector2 drawPos = trailLength[k] - Main.screenPosition;
				Vector2 currentPos = trailLength[k];
				Vector2 betweenPositions = previousPosition - currentPos;

				float max = betweenPositions.Length();

				for (int i = 0; i < max; i++)
				{
					drawPos = previousPosition + -betweenPositions * (i / max) - Main.screenPosition;

					Main.spriteBatch.Draw(ProjTexture.Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
				}

				previousPosition = currentPos;
			}

			return true;
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			int WhoAmI = target.whoAmI;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC NPC = Main.npc[i];
				if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(target.Center, NPC.Center) <= 400f)
				{
					//dont allow the bullet to bounce to the same enemy it already hit
					if (i == WhoAmI)
					{
						continue;
					}

					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);

					Vector2 ChargeDirection = NPC.Center - target.Center;
					ChargeDirection.Normalize();
					ChargeDirection *= 35f;
					Projectile.velocity = ChargeDirection;

					break;
				}
			}
		}

		public override void AI()
        {
            //rotation so the after image draws nicely
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.rotation += 0f * (float)Projectile.direction;

            if (runOnce)
			{
				for (int i = 0; i < trailLength.Length; i++)
				{
					trailLength[i] = Vector2.Zero;
				}
				runOnce = false;
			}

			Vector2 current = Projectile.Center;
			for (int i = 0; i < trailLength.Length; i++)
			{
				Vector2 previousPosition = trailLength[i];
				trailLength[i] = current;
				current = previousPosition;
			}
		}
    }
}
     
          






