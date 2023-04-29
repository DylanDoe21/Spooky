using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientChikProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 500f;
            ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 275f;
            ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 12f;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.MeleeNoSpeed;
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Yoyo;
        }

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] >= 45)
            {
                Vector2 speed = new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-7f, -5f));
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, speed, 
                ModContent.ProjectileType<SentientChikTear>(), Projectile.damage / 2, 0f, Main.myPlayer, 0, 0);
            
                Projectile.localAI[0] = 0;
            }
        }
    }
}
