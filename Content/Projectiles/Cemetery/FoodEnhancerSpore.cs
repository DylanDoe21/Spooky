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
            Projectile.timeLeft = 80;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (Projectile.timeLeft >= 60)
            {
                int DustEffect = Dust.NewDust(Projectile.Center, Projectile.width / 10, Projectile.height / 10, 
                ModContent.DustType<SmokeEffect>(), 0f, 0f, 100, Color.Red * 0.5f, Main.rand.NextFloat(0.2f, 0.4f));
                Main.dust[DustEffect].velocity *= 0;
                Main.dust[DustEffect].alpha = 100;
            }

            for (int i = 0; i <= Main.maxNPCs; i++)
            {
                if (Projectile.Hitbox.Intersects(Main.npc[i].Hitbox))
                {
                    if (Main.npc[i].active && !Main.npc[i].friendly && !Main.npc[i].dontTakeDamage && !NPCID.Sets.CountsAsCritter[Main.npc[i].type])
                    {
                        int[] Tier1Buffs = { BuffID.Poisoned, BuffID.BloodButcherer, BuffID.OnFire };
                        int[] Tier2Buffs = { BuffID.Frostburn, BuffID.ShadowFlame };
                        int[] Tier3Buffs = { BuffID.BetsysCurse, BuffID.Confused, BuffID.Venom };

                        Main.npc[i].AddBuff(Main.rand.Next(Tier1Buffs), 300);

                        if (player.HasBuff(BuffID.WellFed2))
                        {
                            Main.npc[i].AddBuff(Main.rand.Next(Tier2Buffs), 300);
                        }

                        if (player.HasBuff(BuffID.WellFed3))
                        {
                            Main.npc[i].AddBuff(Main.rand.Next(Tier3Buffs), 300);
                        }
                    }
                }
            }
        }
    }
}