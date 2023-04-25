using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class ToiletPaperProj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 28;
            Projectile.height = 26;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 0;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
			Projectile.rotation += 0.1f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 60)
            {
                Projectile.velocity.X = Projectile.velocity.X * 0.99f;
                Projectile.velocity.Y = Projectile.velocity.Y + 0.08f;
            }

            if (Main.rand.Next(45) == 0)
            {
                float Scale = Main.rand.NextFloat(0.5f, 1f);
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.GoreType<Content.Gores.ToiletPaperGore>(), Scale);
            }
        }
		
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                float Scale = Main.rand.NextFloat(0.7f, 1.2f);
                Gore.NewGore(Projectile.GetSource_Death(), new Vector2(Projectile.Center.X + Main.rand.Next(-20, 20), Projectile.Center.Y + Main.rand.Next(-20, 20)), Vector2.Zero, ModContent.GoreType<Content.Gores.ToiletPaperGore>(), Scale);
            }
        }
    }
}