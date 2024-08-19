using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class FlaskCloud : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            switch ((int)Projectile.ai[0])
            {
                case 0: 
                {
                    target.AddBuff(BuffID.Ichor, 300);
                    break;
                }
                case 1: 
                {
                    target.AddBuff(BuffID.Ichor, 300);
                    break;
                }
                case 2: 
                {
                    target.AddBuff(BuffID.Ichor, 300);
                    break;
                }
            }
        }

        public override void AI()
        {
            Projectile.velocity *= 0;

            int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.2f, 0.4f));
            Main.dust[DustEffect].velocity *= 0;
            Main.dust[DustEffect].alpha = 100;

            switch ((int)Projectile.ai[0])
            {
                case 0: 
                {
                    Main.dust[DustEffect].color = new Color(153, 217, 234) * 0.5f;
                    break;
                }
                case 1: 
                {
                    Main.dust[DustEffect].color = new Color(254, 202, 80) * 0.5f;
                    break;
                }
                case 2: 
                {
                    Main.dust[DustEffect].color = new Color(185, 128, 193) * 0.5f;
                    break;
                }
            }
        }
    }
}