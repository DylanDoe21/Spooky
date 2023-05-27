using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class PumpkinHead : ModProjectile
    {   
        int shootTimer = 50;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            
            if (player.dead)
            {
                player.GetModPlayer<SpookyPlayer>().HorsemanSet = false;
            }

            if (player.GetModPlayer<SpookyPlayer>().HorsemanSet)
            {
                Projectile.timeLeft = 2;
                player.AddBuff(ModContent.BuffType<PumpkinHeadBuff>(), 1, false);
            }
            
            if (!player.GetModPlayer<SpookyPlayer>().HorsemanSet)
            {
                Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0f);

            //targetting and shooting
            for (int i = 0; i < 200; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this, false) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(Projectile.Center, Target.Center) <= 450f)
                {
					Shoot(Target);

					break;
				}

				NPC NPC = Main.npc[i];
                if (NPC.active && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 450f)
                {
					Shoot(NPC);

					break;
				}
            }

            //movement
            if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
            {
                Projectile.ai[0] = 1f;
            }

            float speed = 8f;
            if (Projectile.ai[0] == 1f)
            {
                speed = 15f;
            }

            Vector2 center = Projectile.Center;
            Vector2 direction = player.Center - center;
            Projectile.ai[1] = 3600f;
            Projectile.netUpdate = true;
            int num = 1;
            for (int k = 0; k < Projectile.whoAmI; k++)
            {
                if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
                {
                    num++;
                }
            }
            
            direction.Y -= 70f;
            float distanceTo = direction.Length();
            if (distanceTo > 200f && speed < 9f)
            {
                speed = 9f;
            }
            if (distanceTo < 100f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (distanceTo > 2000f)
            {
                Projectile.Center = player.Center;
            }
            if (distanceTo > 48f)
            {
                direction.Normalize();
                direction *= speed;
                float temp = 40 / 2f;
                Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
            }
            else
            {
                Projectile.direction = Main.player[Projectile.owner].direction;
                Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;

            if ((double)Math.Abs(Projectile.velocity.X) > 0.2)
            {
                Projectile.spriteDirection = -Projectile.direction;
                return;
            }
        }

        public void Shoot(NPC target)
		{
			shootTimer++;

			if (shootTimer >= 40)
			{
				SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);

				float Speed = 20f;
				Vector2 vector = new(Projectile.position.X + (Projectile.width / 2), Projectile.position.Y + (Projectile.height / 2));
				float rotation = (float)Math.Atan2(vector.Y - (target.position.Y + (target.height * 0.5f)), vector.X - (target.position.X + (target.width * 0.5f)));
				Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1));

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y,
				perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<PumpkinHeadBolt>(), 30, 0f, Main.myPlayer, 0f, 0f);

                shootTimer = 0;
			}
		}

        public override void Kill(int timeLeft)
		{
			for (int numDust = 0; numDust < 20; numDust++)
            {
                int dust = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 1.5f;
                Main.dust[dust].noGravity = true;
            }
		}
    }
}