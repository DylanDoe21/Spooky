using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.Projectiles.Minibiomes.Desert
{
    public class CactusNeedle : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Minibiomes/Desert/CactusNeedle";

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
			Projectile.penetrate = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            Projectile.ai[0]++;
			if (Projectile.ai[0] > 35)
			{
                Projectile.velocity.Y = Projectile.velocity.Y + 0.45f;
            }
        }

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Dig with { Volume = 0.45f }, Projectile.Center);
		}
    }
}