using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class Brainy : ModProjectile
    {
        float ScaleAmount = 0.05f;
        int ScaleTimerLimit = 10;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brainy");
            Main.projFrames[Projectile.type] = 7;
            Main.projPet[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 46;
            Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
            Projectile.minionSlots = 1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] <= 120)
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 6)
                    {
                        Projectile.frame = 0;
                    }
                }
            }
            else
            {
                Projectile.frame = 6;
            }

            Player player = Main.player[Projectile.owner];

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().Brainy = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().Brainy)
			{
				Projectile.timeLeft = 2;
			}

            //actual minion exploding ai
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] >= 120 && Projectile.localAI[0] < 240)
            {
                if (Projectile.localAI[0] == 120 || Projectile.localAI[0] == 150 || Projectile.localAI[0] == 180)
                {
                    ScaleAmount += 0.05f;
                    ScaleTimerLimit -= 3;
                }

                Projectile.localAI[1]++;
                if (Projectile.localAI[1] < ScaleTimerLimit)
                {
                    Projectile.scale -= ScaleAmount;
                }
                if (Projectile.localAI[1] >= ScaleTimerLimit)
                {
                    Projectile.scale += ScaleAmount;
                }

                if (Projectile.localAI[1] > ScaleTimerLimit * 2)
                {
                    Projectile.localAI[1] = 0;
                    Projectile.scale = 1f;
                }
            }
            else
            {   
                Projectile.localAI[1] = 0;
                Projectile.scale = 1f;
            }

            if (Projectile.localAI[0] >= 240)
            {
                for (int k = 0; k < Main.projectile.Length; k++)
                {
                    if (Main.projectile[k].active && Main.projectile[k].minion) 
                    {
                        SoundEngine.PlaySound(SoundID.Item96, Main.projectile[k].Center);

                        for (int numDust = 0; numDust < 20; numDust++)
                        {
                            int newDust = Dust.NewDust(Main.projectile[k].Center, Main.projectile[k].width, Main.projectile[k].height, 
                            DustID.PurpleCrystalShard, 0f, 0f, 100, default(Color), 2f);

                            Main.dust[newDust].scale *= 0.5f;
                            Main.dust[newDust].noGravity = true;

                            if (Main.rand.Next(2) == 0)
                            {
                                Main.dust[newDust].scale = 0.5f;
                                Main.dust[newDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                            }
                        }

                        Main.projectile[k].Kill();
                    }
                }

                ScaleAmount = 0.05f;
                ScaleTimerLimit = 10;

                Projectile.localAI[0] = 0;
            }
        
            //movement stuff
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
    }
}