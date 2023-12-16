using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Boss.Orroboro.Projectiles
{
    public class FleshPillarTelegraph : ModProjectile
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public static readonly SoundStyle BoneSnap = new SoundStyle("Spooky/Content/Sounds/Orroboro/BoneSnap", SoundType.Sound);

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 3600;  
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            if (Projectile.ai[0] <= 60)
            {
                int dirtDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, 0f, -2f, 0, default, 4f);
                Main.dust[dirtDust].noGravity = true;
                Main.dust[dirtDust].velocity.Y *= Main.rand.Next(10, 25);
            }
            if (Projectile.ai[0] == 40)
            {
                Vector2 lineDirection = new Vector2(0, 16);
                
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y - 20, 0, 0,
                ModContent.ProjectileType<FleshPillar>(), Projectile.damage, 0, Main.myPlayer, lineDirection.ToRotation() + MathHelper.Pi, -16 * 60);
            }
            if (Projectile.ai[0] >= 80)
            {
                SoundEngine.PlaySound(BoneSnap, Projectile.position);

                Projectile.Kill();
            }
        }
    }
}