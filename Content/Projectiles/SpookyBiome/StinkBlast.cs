using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class StinkBlast : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {   
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 75;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            target.AddBuff(BuffID.Stinky, 600);
        }
        
        public override void AI()
        {
            Color color = new Color(114, 103, 42); 

            switch (Main.rand.Next(3))
            {
                //brown
                case 0:
                {
                    color = new Color(114, 103, 42);
                    break;
                }
                //dark orange
                case 1:
                {
                    color = new Color(145, 100, 29);
                    break;
                }
                //reddish orange
                case 2:
                {
                    color = new Color(178, 67, 46);
                    break;
                }
            }

            int DustEffect = Dust.NewDust(Projectile.Center, Projectile.width / 5, Projectile.height / 5, 
            ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, color * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
            Main.dust[DustEffect].velocity *= 0;
            Main.dust[DustEffect].alpha = 100;

            if (Main.rand.NextBool(10))
            {
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].position.X += Main.rand.Next(-50, 50) * 0.05f - 1.5f;
                Main.dust[dust].position.Y += Main.rand.Next(-50, 50) * 0.05f - 1.5f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-3, 3), Main.rand.Next(-3, 3), 
                ModContent.ProjectileType<DecayDebuffFly>(), 0, 0f, Main.myPlayer, 0, 0);
            }
        }
    }
}