using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class TarTomeFeather : ModProjectile
    {
        public bool IsStickingToTarget = false;

		Vector2 RotatePosition;

        private static Asset<Texture2D> ProjTexture;

        public override void SetDefaults()
        {
			Projectile.width = 36;
            Projectile.height = 42;     
			Projectile.friendly = true;                              			  		
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsStickingToTarget && target.HasBuff(ModContent.BuffType<Tarred>()))
            {
                Projectile.timeLeft = 60;
                Projectile.ai[2] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
        }
		
		public override void AI()
        {
			if (IsStickingToTarget)
			{
				Projectile.frame = 3;

				Projectile.ignoreWater = true;
				Projectile.tileCollide = false;

				int npcTarget = (int)Projectile.ai[2];
				if (npcTarget < 0 || npcTarget >= 200)
				{
					Projectile.Kill();
				}
				else if (Main.npc[npcTarget].active && !Main.npc[npcTarget].dontTakeDamage)
				{
					Projectile.Center = Main.npc[npcTarget].Center - Projectile.velocity * 2f;
					Projectile.gfxOffY = Main.npc[npcTarget].gfxOffY;
				}
				else
				{
					Projectile.Kill();
				}
			}
			else
			{
				Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
				Projectile.rotation += 0f * (float)Projectile.direction;

				Projectile.ai[0]++;

				if (Projectile.ai[0] == 1)
				{
					Projectile.localAI[0] = Main.rand.Next(30, 61);
				}
				if (Projectile.ai[0] == Projectile.localAI[0])
				{
					RotatePosition = new Vector2(Projectile.Center.X, Projectile.Center.Y + (Main.rand.NextBool() ? -15 : 15));
				}
				if (Projectile.ai[0] > Projectile.localAI[0])
				{
					double angle = Projectile.DirectionTo(RotatePosition).ToRotation() - Projectile.velocity.ToRotation();
					while (angle > Math.PI)
					{
						angle -= 2.0 * Math.PI;
					}
					while (angle < -Math.PI)
					{
						angle += 2.0 * Math.PI;
					}

					if (Math.Abs(angle) > Math.PI / 2)
					{
						Projectile.ai[1] = Math.Sign(angle);
						Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 5;
					}

					Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(12f) * Projectile.ai[1]);
				}
				if (Projectile.ai[0] >= Projectile.localAI[0] + 60)
				{
					Projectile.ai[0] = 0;
				}
			}

			if (Projectile.timeLeft <= 60)
            {
                Projectile.scale *= 0.95f;
            }
		}
	}
}