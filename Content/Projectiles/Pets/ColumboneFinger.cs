using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Pets
{
    public class ColumboneFinger : ModProjectile
    {
        public static readonly SoundStyle VineBoomSound = new("Spooky/Content/Sounds/ColumboThud", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 500;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(VineBoomSound, Projectile.Center);
            }

            if (Projectile.ai[0] >= 15)
            {
                Projectile.Kill();
            }
        }
    }
}