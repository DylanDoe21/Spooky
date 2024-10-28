using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class TortumorYoyoProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 12f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 275f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] % 180 == 0 && player.ownedProjectileCounts[ModContent.ProjectileType<TortumorYoyoChunk>()] < 3)
            {
                Vector2 speed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));

                if (Main.netMode != NetmodeID.MultiplayerClient)
			    {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, speed, ModContent.ProjectileType<TortumorYoyoChunk>(), 
                    Projectile.damage / 2, Projectile.knockBack, player.whoAmI, Projectile.whoAmI);
                }
            }
        }
    }
}
