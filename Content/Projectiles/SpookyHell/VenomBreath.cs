using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class VenomBreath : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {   
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 75;
            Projectile.extraUpdates = 3;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            target.AddBuff(BuffID.Venom, 120);
        }
        
        public override void AI()
        {
            if (Main.rand.NextBool(3))
            {
                int DustEffect = Dust.NewDust(Projectile.Center, Projectile.width / 5, Projectile.height / 5, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.BlueViolet * 0.5f, Main.rand.NextFloat(0.2f, 0.5f));
                Main.dust[DustEffect].velocity *= 0;
                Main.dust[DustEffect].alpha = 150;
            }
        }
    }
}