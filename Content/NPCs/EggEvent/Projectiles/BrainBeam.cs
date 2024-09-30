using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
    public class BrainBeam : ModProjectile
    {
		public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
			Projectile.width = 20;
            Projectile.height = 20;
			Projectile.friendly = false;
			Projectile.hostile = true;                               			  		
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;  
            Projectile.penetrate = 2;
			Projectile.timeLeft = 300;
            Projectile.alpha = 255;
		}

		public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.Confused, 60);
        }

		public override void AI()
        {
            Projectile.rotation += 0.5f * (float)Projectile.direction;

			int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<BrainConfusionDust>(), 0f, 0f, 100, Color.White * 0.75f, 1f);
			Main.dust[DustEffect].velocity *= 0;
		}
    }
}
     
          






