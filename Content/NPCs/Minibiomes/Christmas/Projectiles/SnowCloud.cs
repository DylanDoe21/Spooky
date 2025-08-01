using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Minibiomes.Christmas.Projectiles
{
    public class SnowCloud : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 130;
            Projectile.height = 130;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 3;
            Projectile.alpha = 255;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            int[] Debuffs = new int[] { BuffID.Confused, BuffID.Weak, BuffID.Chilled, BuffID.Suffocation };
            
            target.AddBuff(Main.rand.Next(Debuffs), Main.rand.Next(240, 361));
        }

        public override void AI()
        {
            int DustEffect = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CoughCloudDust>(), 0f, 0f, 100, Color.White * 0.25f, Main.rand.NextFloat(0.3f, 0.6f));
            Main.dust[DustEffect].velocity = Vector2.Zero;
            Main.dust[DustEffect].alpha = 100;
        }
    }
}