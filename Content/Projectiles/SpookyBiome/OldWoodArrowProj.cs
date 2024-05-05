using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class OldWoodArrowProj : ModProjectile
    {
        float SaveRotation;

		public bool IsStickingToTarget = false;
		
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1200;
			Projectile.penetrate = -1;
        }

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsStickingToTarget && Main.rand.NextBool(4))
            {
                SaveRotation = Projectile.rotation;

                Projectile.timeLeft = 60;
                Projectile.ai[1] = target.whoAmI;
                Projectile.velocity = (target.Center - Projectile.Center) * 0.75f;
                IsStickingToTarget = true;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.Kill();
            }
        }

        public override void AI()       
        {
			if (IsStickingToTarget) 
            {
                Projectile.rotation = SaveRotation;

				Projectile.ignoreWater = true;
                Projectile.tileCollide = false;

                int npcTarget = (int)Projectile.ai[1];
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

            	Projectile.velocity.Y = Projectile.velocity.Y + 0.15f;
			}
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

			return true;
		}

		public override void OnKill(int timeLeft)
		{
            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/OldWoodArrowGore" + numGores).Type);
                }
            }

			for (int numDusts = 0; numDusts < 10; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, 0f, -2f, 0, default, 1f);
				Main.dust[dust].position.X += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-25, 25) * 0.05f - 1.5f;
				Main.dust[dust].noGravity = true;

				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}