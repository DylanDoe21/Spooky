using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class PumpkinHead : ModProjectile
    {   
        int shootTimer = 50;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pumpkin Head");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
            Projectile.width = 24;
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
            SpookyPlayer modPlayer = player.GetModPlayer<SpookyPlayer>();
            
            if (player.dead)
            {
                modPlayer.SpookySet = false;
            }
            if (modPlayer.SpookySet)
            {
                Projectile.timeLeft = 2;
                player.AddBuff(ModContent.BuffType<PumpkinHeadBuff>(), 1, false);
            }
            if (!modPlayer.SpookySet)
            {
                Projectile.Kill();
            }

            Lighting.AddLight(Projectile.Center, 0.6f, 0.3f, 0f);

            shootTimer++;

            for (int i = 0; i < 200; i++)
            {
                NPC NPC = Main.npc[i];
                if (NPC.active && !NPC.friendly && NPC.damage > 0 && !NPC.dontTakeDamage && Vector2.Distance(Projectile.Center, NPC.Center) <= 400f)
                {
                    if (shootTimer >= 40)
                    {
                        float Speed = 8f;
                        Vector2 vector = new(Projectile.position.X + (Projectile.width / 2), Projectile.position.Y + (Projectile.height / 2));
                        float rotation = (float)Math.Atan2(vector.Y - (NPC.position.Y + (NPC.height * 0.5f)), vector.X - (NPC.position.X + (NPC.width * 0.5f)));
                        Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1)).RotatedByRandom(MathHelper.ToRadians(20));
                        
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 
                        perturbedSpeed.X, perturbedSpeed.Y, ModContent.ProjectileType<PumpkinHeadSeed>(), 30, 0f, Main.myPlayer, 0f, 0f);

                        shootTimer = 0;
                    }
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
            //direction.X -= (float)((10 + num * 40) * player.direction);
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

        public override void Kill(int timeLeft)
		{
			for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 288, 0f, 0f, 100, default, 2f);
                Main.dust[DustGore].velocity *= 1.5f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
		}
    }
}