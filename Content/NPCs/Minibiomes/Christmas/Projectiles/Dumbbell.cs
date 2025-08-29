using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{ 
    public class Dumbbell : ModProjectile
    {
        public override void SetDefaults()
        {
			Projectile.width = 36;
            Projectile.height = 16;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SoundID.Tink with { Volume = 0.45f }, Projectile.Center);

                Projectile.velocity = Vector2.Zero;

                Projectile.ai[0]++;
            }

			return false;
		}
		
		public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
			    Projectile.rotation += 0.2f * (float)Projectile.direction;
            }

            Projectile.velocity.Y = Projectile.velocity.Y + 1.75f;
        }
    }
}