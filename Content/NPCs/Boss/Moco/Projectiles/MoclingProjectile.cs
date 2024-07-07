using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Boss.Moco.Projectiles
{
	public class MoclingProjectile : ModProjectile
	{
        public override string Texture => "Spooky/Content/NPCs/SpookyHell/Mocling";

        Vector2 SaveProjPosition;

        private static Asset<Texture2D> ProjTexture;

        public static readonly SoundStyle FlyingSound = new("Spooky/Content/Sounds/Moco/MocoFlying", SoundType.Sound) { Pitch = 0.45f };

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 26;
			Projectile.friendly = false;
            Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 240;
            Projectile.penetrate = 3;
            Projectile.aiStyle = -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);

            Color color = new Color(125 - Projectile.alpha, 125 - Projectile.alpha, 125 - Projectile.alpha, 0).MultiplyRGBA(Color.Lime);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

            for (int oldPos = 0; oldPos < Projectile.oldPos.Length; oldPos++)
            {
                var effects = Projectile.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                float scale = Projectile.scale * (Projectile.oldPos.Length - oldPos) / Projectile.oldPos.Length * 1f;
                Vector2 drawPos = Projectile.oldPos[oldPos] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Rectangle rectangle = new(0, (ProjTexture.Height() / Main.projFrames[Projectile.type]) * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                Main.EntitySpriteDraw(ProjTexture.Value, drawPos, rectangle, Projectile.GetAlpha(color), Projectile.oldRot[oldPos], drawOrigin, scale * 1.1f, effects, 0);
            }

            return true;
        }

		public override void AI()
		{
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 2)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 8)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? -1 : 1;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (Projectile.spriteDirection == 1)
            {
                Projectile.rotation += MathHelper.Pi;
            }

            Projectile.ai[0]++;

            if (Projectile.ai[0] < 10)
            {
                Vector2 GoTo = player.Center + new Vector2(Main.rand.Next(-250, 250), Main.rand.Next(-400, -350));

                if (Projectile.Distance(GoTo) >= 200f)
                { 
                    GoTo -= Projectile.DirectionTo(GoTo) * 100f;
                }

                Vector2 GoToVelocity = GoTo - Projectile.Center;

                float lerpValue = Utils.GetLerpValue(100f, 600f, GoToVelocity.Length(), false);

                float velocityLength = GoToVelocity.Length();

                if (velocityLength > 18f)
                { 
                    velocityLength = 18f;
                }

                Projectile.velocity = Vector2.Lerp(GoToVelocity.SafeNormalize(Vector2.Zero) * velocityLength, GoToVelocity / 6f, lerpValue);
            }

            if (Projectile.ai[0] >= 10 && Projectile.ai[0] <= 40)
            {
                Projectile.velocity *= 0.85f;
            }
            
            if (Projectile.ai[0] == 45)
            {
                SaveProjPosition = Projectile.Center;
            }

            if (Projectile.ai[0] > 45 && Projectile.ai[0] <= 55)
            {
                Projectile.Center = new Vector2(SaveProjPosition.X, SaveProjPosition.Y);
                Projectile.Center += Main.rand.NextVector2Square(-6, 6);
            }

            if (Projectile.ai[0] == 60)
            {
                Vector2 ChargeDirection = player.Center - Projectile.Center;
                ChargeDirection.Normalize();
                        
                ChargeDirection.X *= 1f;
                ChargeDirection.Y *= 2f;
                Projectile.velocity.X = ChargeDirection.X;
                Projectile.velocity.Y = ChargeDirection.Y;
            }

            if (Projectile.ai[0] > 60)
            {
                Projectile.velocity *= 1.068f;

                Projectile.tileCollide = true;
            }
		}

        public override void OnKill(int timeLeft)
		{
            SoundEngine.PlaySound(SoundID.NPCDeath16, Projectile.Center);

        	if (Main.netMode != NetmodeID.Server) 
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.Center, Projectile.velocity, ModContent.Find<ModGore>("Spooky/MoclingProjectileGore").Type);
            }
		}
	}
}