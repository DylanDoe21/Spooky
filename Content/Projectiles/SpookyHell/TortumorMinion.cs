using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class TortumorMinion : ModProjectile
    {   
        int shootTimer = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 58;
            Projectile.height = 62;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
			Projectile.minionSlots = 1;
            Projectile.aiStyle = 62;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 30)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 6)
                {
                    Projectile.frame = 0;
                }
            }
			
            return true;
        }

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().TumorMinion = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().TumorMinion)
			{
				Projectile.timeLeft = 2;
			}

			for (int i = 0; i < 200; i++)
            {
				NPC Target = Projectile.OwnerMinionAttackTargetNPC;
				if (Target != null && Target.CanBeChasedBy(this, false) && !NPCID.Sets.CountsAsCritter[Target.type])
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

            //prevent Projectiles clumping together
			for (int num = 0; num < Main.projectile.Length; num++)
			{
				Projectile other = Main.projectile[num];
				if (num != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
				{
					const float pushAway = 0.08f;
					if (Projectile.position.X < other.position.X)
					{
						Projectile.velocity.X -= pushAway;
					}
					else
					{
						Projectile.velocity.X += pushAway;
					}
					if (Projectile.position.Y < other.position.Y)
					{
						Projectile.velocity.Y -= pushAway;
					}
					else
					{
						Projectile.velocity.Y += pushAway;
					}
				}
			}
		}

		public void Shoot(NPC target)
		{
			shootTimer++;

			if (shootTimer == 40 || shootTimer == 50)
			{
				SoundEngine.PlaySound(SoundID.Item87, Projectile.Center);

				int[] Projectiles = new int[] { ModContent.ProjectileType<TortumorMinionOrb1>(), ModContent.ProjectileType<TortumorMinionOrb2>() };

				float Speed = 20f;
				Vector2 vector = new(Projectile.position.X + (Projectile.width / 2), Projectile.position.Y + (Projectile.height / 2));
				float rotation = (float)Math.Atan2(vector.Y - (target.position.Y + (target.height * 0.5f)), vector.X - (target.position.X + (target.width * 0.5f)));
				Vector2 perturbedSpeed = new Vector2((float)((Math.Cos(rotation) * Speed) * -1), (float)((Math.Sin(rotation) * Speed) * -1)).RotatedByRandom(MathHelper.ToRadians(20));

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y,
				perturbedSpeed.X, perturbedSpeed.Y, Main.rand.Next(Projectiles), Projectile.damage, 0f, Main.myPlayer, 0f, 0f);
			}

			if (shootTimer >= 60)
			{
				shootTimer = 0;
			}
		}

		public override void Kill(int timeLeft)
		{
            for (int numDusts = 0; numDusts < 10; numDusts++)
			{                                                                                  
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;

				if (Main.dust[dust].position != Projectile.Center)
                {
				    Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
                }
			}
        }
    }
}