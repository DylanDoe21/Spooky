using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class EarWormSoundBase : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
            Projectile.extraUpdates = 3;
            Projectile.alpha = 255;
        }

        public override bool? CanDamage()
        {
			return false;
        }

        public override bool CanHitPlayer(Player target)
		{
			return false;
		}

        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] % 8 == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EarWormSound>(), Projectile.damage, 5f, Main.myPlayer);
            }
        }
    }
}