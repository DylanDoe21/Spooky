using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

using Spooky.Content.Dusts;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class FoodEnhancerSpore : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];

            int[] Tier1Buffs = { BuffID.Poisoned, BuffID.Bleeding };
            int[] Tier2Buffs = { BuffID.Confused, BuffID.Venom };
            int[] Tier3Buffs = { BuffID.BetsysCurse, BuffID.Frostburn2 };

            target.AddBuff(Main.rand.Next(Tier1Buffs), 300);

            if (player.HasBuff(BuffID.WellFed2))
            {
                target.AddBuff(Main.rand.Next(Tier2Buffs), 300);
            }

            if (player.HasBuff(BuffID.WellFed3))
            {
                target.AddBuff(Main.rand.Next(Tier3Buffs), 300);
            }
        }

        public override void AI()
        {
            Projectile.velocity *= 0.95f;

            int DustEffect = Dust.NewDust(Projectile.Center, Projectile.width / 10, Projectile.height / 10, 
            ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.5f, Main.rand.NextFloat(0.2f, 0.4f));
            Main.dust[DustEffect].velocity *= 0;
            Main.dust[DustEffect].alpha = 100;
        }
    }
}