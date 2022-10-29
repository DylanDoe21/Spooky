using Terraria.ModLoader;

namespace Spooky.Content.NPCs.Boss.RotGourd.Projectiles
{
    public class RotStomp : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rot Gourd's Slam");
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(683);
            AIType = 683;
            Projectile.width = 100;
            Projectile.height = 20;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 60;
        }
    }
}