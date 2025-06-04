using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.SpiderCave
{
    public class SpiderSummonEggProj : ModProjectile
    {
        public override string Texture => "Spooky/Content/Items/SpiderCave/SpiderSummonEgg";

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
		{
            return false;
        }

        public override void AI()
        {
			Projectile.rotation += 0.25f * (float)Projectile.direction;

            Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
        }

        public override void OnKill(int timeLeft)
		{
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.Center);

            player.AddBuff(ModContent.BuffType<SpiderBabyBuff>(), 2);

            for (int numSpiders = 0; numSpiders < 2; numSpiders++)
			{
                int[] PotentialSpiders = { ModContent.ProjectileType<SpiderBabyGreen>(), ModContent.ProjectileType<SpiderBabyPurple>(), ModContent.ProjectileType<SpiderBabyRed>() };

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - Main.rand.Next(0, 10)), 
                Vector2.Zero, Main.rand.Next(PotentialSpiders), Projectile.damage, 0f, Projectile.owner);
            }

            for (int numGores = 1; numGores <= 3; numGores++)
			{
				if (Main.netMode != NetmodeID.Server) 
				{
					Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/SpiderEggGore" + numGores).Type);
				}
			}

            for (int numDusts = 0; numDusts < 10; numDusts++)
			{
				int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Web, 0f, -2f, 0, default, 1.5f);
				Main.dust[dust].noGravity = true;
				Main.dust[dust].position.X += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
				Main.dust[dust].position.Y += Main.rand.Next(-35, 35) * 0.05f - 1.5f;
					
				if (Main.dust[dust].position != Projectile.Center)
				{
					Main.dust[dust].velocity = Projectile.DirectionTo(Main.dust[dust].position) * 2f;
				}
			}
		}
    }
}