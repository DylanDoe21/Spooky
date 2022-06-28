using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
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
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 999999999;
            Projectile.timeLeft *= 999999999;
            Projectile.penetrate = -1;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 16)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

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

            shootTimer--;
            float max = 400f;
            Projectile.tileCollide = false;
            for (int i = 0; i < 200; i++)
            {
                NPC NPC = Main.npc[i];
                if (NPC.active && !NPC.friendly && NPC.damage > 0 && !NPC.dontTakeDamage && Vector2.Distance(Projectile.Center, NPC.Center) <= max)
                {
                    int numberProjectiles = 1;
                    Vector2 vector8 = new(Projectile.position.X + (Projectile.width / 2), Projectile.position.Y + (Projectile.height / 2));
                    int type = ProjectileID.AmberBolt;
                    float Speed = 12f;
                    float rotation = (float)Math.Atan2(vector8.Y - (NPC.position.Y + (NPC.height * 0.5f)), vector8.X - (NPC.position.X + (NPC.width * 0.5f)));
                    int damage = 20;
                    if (shootTimer <= 0)
                    {
                        for (int l = 0; l < numberProjectiles; l++)
                        {
                            Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1)).RotatedByRandom(MathHelper.ToRadians(20));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), vector8.X, vector8.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        shootTimer = 50;
                    }
                }
            }

            //make cool fire dust effect from the pumpkins eye
            Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity);

            Dust newDust = Main.dust[Dust.NewDust(Projectile.position, (Projectile.spriteDirection == -1 ? Projectile.width + 13 : Projectile.width - 28), 
            Projectile.height + 40, DustID.Torch, 0f, 0f, 0, default, 1f)];

            newDust.position = position;
            newDust.velocity = Projectile.velocity.RotatedBy(Math.PI / 2, default) * 0.33F + Projectile.velocity / 4;
            newDust.fadeIn = 0.5f;
            newDust.noGravity = true;

            //movement code, copied from moco pet lol
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
			for (int i = 0; i < 10; i++)
			{                                                                                  
				int newDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DemonTorch, 0f, -2f, 0, default, 1.5f);
				Main.dust[newDust].noGravity = true;
				Main.dust[newDust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[newDust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[newDust].position != Projectile.Center)
                {
				    Main.dust[newDust].velocity = Projectile.DirectionTo(Main.dust[newDust].position) * 2f;
                }
			}
		}
    }
}