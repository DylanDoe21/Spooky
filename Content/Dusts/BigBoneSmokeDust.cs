using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Dusts
{
    public class BigBoneSmokeDust : ModDust
    {
        public override string Texture => "Spooky/Content/Dusts/CoughCloudDust";

        public override void OnSpawn(Dust dust)
        {
            dust.noGravity = true;
            dust.scale *= Main.rand.NextFloat(0.8f, 2f);
            dust.frame = new Rectangle(0, 0, 36, 34);
            dust.rotation = Main.rand.NextFloat(6.28f);
        }

        public override bool Update(Dust dust)
        {
            dust.color *= 0.99f;

            dust.scale *= 1.002f;
            dust.alpha += 3;

            dust.position += dust.velocity;

            if (dust.alpha >= 255)
            {
                dust.active = false;
            }

            return false;
        }
    }
}