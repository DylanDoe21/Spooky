using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace Spooky.Content.Projectiles.SpookyBiome
{
    public class RangedEgg : ModProjectile
    {
        public static readonly SoundStyle EggCrackSound = new("Spooky/Content/Sounds/Orroboro/EggCrack1", SoundType.Sound) { Pitch = 1.5f };

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 2000;
            Projectile.penetrate = 1;
            Projectile.aiStyle = 0;
        }

        public override void AI()
        {
			Projectile.rotation += 0.2f * (float)Projectile.direction;

            Projectile.ai[0]++;
            if (Projectile.ai[0] >= 15)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.35f;
            }
        }
		
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(EggCrackSound, Projectile.Center);

            for (int numGores = 1; numGores <= 2; numGores++)
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/RangedEggGore" + numGores).Type);
                }
            }
        }
    }
}