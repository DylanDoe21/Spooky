using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Blooms
{
	public class IrisPetalLockOn : ModProjectile
    {
        public static readonly SoundStyle DeathSound = new("Spooky/Content/Sounds/IrisEyePoke", SoundType.Sound);

        public override void SetDefaults()
        {
			Projectile.width = 68;
            Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
			return false;
        }
        
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 5;

            Projectile.position = new Vector2(Main.MouseWorld.X - (Projectile.width / 2), Main.MouseWorld.Y - (Projectile.height / 2));

            if (player.dead || !player.GetModPlayer<BloomBuffsPlayer>().SpringIris)
            {
                Projectile.Kill();
            }

            //fade in
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 5;
            }

            //spawn the iris petal projectile and set this as its parent
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 60)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), player.Center, Vector2.Zero, ModContent.ProjectileType<IrisPetal>(), 50, 0, player.whoAmI, Projectile.whoAmI);
            }
        }
    }
}