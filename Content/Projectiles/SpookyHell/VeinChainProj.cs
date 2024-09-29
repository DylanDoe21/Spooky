using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class VeinChainProj : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 12;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
			Projectile.hide = true;
			Projectile.timeLeft = 300;
            Projectile.penetrate = -1;
        }

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			ProjTexture ??= ModContent.Request<Texture2D>(Texture);

			bool flip = false;
			if (Parent.direction == -1)
			{
				flip = true;
			}

			Vector2 drawOrigin = new Vector2(0, ProjTexture.Height() / 2);
			Vector2 myCenter = Projectile.Center - new Vector2(0 * (flip ? -1 : 1), 5).RotatedBy(Parent.rotation);
			Vector2 p0 = player.Center;
			Vector2 p1 = player.Center;
			Vector2 p2 = myCenter - new Vector2(45 * (flip ? -1 : 1), 75).RotatedBy(Parent.rotation);
			Vector2 p3 = myCenter;

			int segments = 32;

			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;
				Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				Vector2 toNext = drawPosNext - drawPos2;
				float rotation = toNext.ToRotation();
				float distance = toNext.Length();

				Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

				Main.spriteBatch.Draw(ProjTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ProjTexture.Width(), 1), SpriteEffects.None, 0f);
			}

			return false;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
        {
			Player player = Main.player[Projectile.owner];

			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			if (player.Distance(Parent.Center) >= 650f)
			{
				Projectile.ai[0] = 1;
			}

			if (Projectile.ai[0] == 0 && Parent.active)
			{
				Projectile.timeLeft = 300;

				Projectile.Center = Parent.Center;

				Projectile.ai[1]++;

				if (Projectile.ai[1] >= 120)
				{
					player.ApplyDamageToNPC(Parent, Projectile.damage, 0, 0, false, null, true);

					int LifeHealed = 10;
					player.statLife += LifeHealed;
					player.HealEffect(LifeHealed, true);

					Projectile.ai[1] = 0;
				}
			}
			else
			{
				Parent.GetGlobalNPC<NPCGlobal>().HasVeinChainAttached = false;

				Vector2 RetractSpeed = Projectile.Center - player.Center;
				RetractSpeed.Normalize();
				RetractSpeed *= -35;
				Projectile.velocity = RetractSpeed;

				if (Projectile.Distance(player.Center) <= 50f)
				{
					Projectile.Kill();
				}
			}
		}
    }
}