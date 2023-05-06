using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
    public class FogDust : ModDust
    {
        public override void OnSpawn(Dust dust)
        {
            dust.velocity *= 0.05f;
            dust.velocity.Y *= 0.5f;
            dust.noGravity = true;
            dust.noLight = true;
            dust.frame = new Rectangle((Main.rand.Next(0, 1) == 0) ? 0 : 250, 0, 250, 115);
            dust.alpha = 200;
            dust.fadeIn = 12f;
            dust.scale *= Main.rand.NextFloat(0.75f, 1.5f);
        }

        public override bool Update(Dust dust)
        {
            if (Main.rand.NextBool(20)) 
            {
                dust.alpha++;
            }
            
            if (Main.rand.NextBool(12)) 
            {
                dust.velocity.Y += Main.rand.NextFloat(0.02f, 0.08f);
            }

            dust.position -= dust.velocity;
            
            if (dust.alpha > 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}