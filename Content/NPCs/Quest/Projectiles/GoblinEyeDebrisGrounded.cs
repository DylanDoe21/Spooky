using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Spooky.Content.NPCs.Quest.Projectiles
{
    public class GoblinEyeDebrisGrounded : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 18;
            Projectile.friendly = false;
			Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 500;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Brown);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int numEffect = 0; numEffect < 2; numEffect++)
            {
                Vector2 vector = new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 3) + (numEffect / 2 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            }

            return true;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return true;
		}

        public override void AI()
        {
            NPC Parent = Main.npc[(int)Projectile.ai[1]];

            if (!Parent.active || Parent.type != ModContent.NPCType<FrankenGoblin>())
            {
                Projectile.Kill();
            }

            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 5;
            }

            Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

            if (Projectile.ai[0] == 0)
            {
                for (int numDusts = 0; numDusts <= 5; numDusts++)
                {
                    int dirtDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default, 2.5f);
                    Main.dust[dirtDust].noGravity = true;
                    Main.dust[dirtDust].velocity.Y *= Main.rand.Next(2, 3);
                }

                Projectile.ai[0]++;
            }

            if (Parent.localAI[3] == 3 && Parent.localAI[1] > 0)
            {
                int Eye = Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y - 45, Main.rand.Next(-2, 3), Main.rand.Next(-18, -12), ModContent.ProjectileType<GoblinEyeDebris>(), Projectile.damage, 0, Main.myPlayer);
                Main.projectile[Eye].frame = Projectile.frame;

                Projectile.active = false;
            }
        }
    }
}