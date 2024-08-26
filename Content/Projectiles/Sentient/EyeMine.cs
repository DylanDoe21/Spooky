using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.Projectiles.Sentient
{
    public class EyeMine : ModProjectile
    {
        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle SplatSound = new("Spooky/Content/Sounds/Splat", SoundType.Sound) { Volume = 0.5f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 18;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            if (Projectile.ai[0] == 0)
            {
                Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

                for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
                {
                    float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                    Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                    Color color = Projectile.GetAlpha(Color.Lerp(Color.Red, Color.Blue, oldPos / (float)Projectile.oldPos.Length)) * ((Projectile.oldPos.Length - oldPos) / (float)Projectile.oldPos.Length);
                    Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, color, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
                }
            }

            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
		{
            if (Projectile.ai[0] == 0)
            {
                SoundEngine.PlaySound(SplatSound, Projectile.Center);

                Projectile.velocity.X *= 0;

                Projectile.ai[0]++;
            }

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) 
		{
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);

			hit.Damage = 0;

            //explosion
            int Multiplier = Projectile.ai[0] == 0 ? 1 : 2;

            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EyeMineExplosion>(), Projectile.damage * Multiplier, 0f, Main.myPlayer);

            //spawn blood splatter
            for (int i = 0; i < 3; i++)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-7, 8), Main.rand.Next(-7, 8), ModContent.ProjectileType<RedSplatter>(), 0, 0);
                }
            }
        }

        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.rotation += 0.5f * (float)Projectile.direction;

                Projectile.velocity.Y = Projectile.velocity.Y + 0.5f;

                Projectile.timeLeft = 300;
            }
            else
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.75f;

                Projectile.rotation = 0;
            }

            Projectile.frame = (int)Projectile.ai[0];

            if (Projectile.timeLeft <= 60)
            {
                Projectile.alpha += 5;
            }
        }
    }
}