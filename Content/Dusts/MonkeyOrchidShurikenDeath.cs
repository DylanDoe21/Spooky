using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
    public class MonkeyOrchidShurikenDeath : ModDust
    {
        public override string Texture => "Spooky/Content/Projectiles/Blooms/MonkeyOrchidShuriken";

        public override void OnSpawn(Dust dust)
        {
            dust.noLight = false;
            dust.frame = new Rectangle(0, 0, 22, 24);
        }

        public override Color? GetAlpha(Dust dust, Color lightColor)
        {
            return lightColor;
        }

        public override bool Update(Dust dust)
        {
            dust.velocity.Y += 0.12f;
            dust.scale *= 0.92f;

            dust.position += dust.velocity;

			dust.rotation += 0.2f * dust.velocity.X;

            if (dust.scale < 0.05f)
            {
                dust.active = false;
            }

            return false;
        }
    }
}