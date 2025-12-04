using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.SpiderCave.Projectiles
{
    public class CorklidRocket : ModProjectile
    {
        float Opacity = 0f;

        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> TrailTexture;

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 90;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 70;
			Projectile.penetrate = -1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            TrailTexture ??= ModContent.Request<Texture2D>(Texture + "Trail");

            Color color = new Color(125, 125, 125, 0).MultiplyRGBA(Color.OrangeRed);

            Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);
			Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);

            for (int i = 0; i < 360; i += 60)
            {
                Vector2 circular = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f)).RotatedBy(MathHelper.ToRadians(i));

                Main.EntitySpriteDraw(TrailTexture.Value, vector + circular, rectangle, color * Opacity, Projectile.rotation, drawOrigin, 1f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height)];

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.rotation += 0f * (float)Projectile.direction;

            if (Opacity < 1f)
            {
                Opacity += 0.05f;
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] < 60)
                {
                    Projectile.velocity.Y = Projectile.velocity.Y + 0.05f;
                }
            }
            else
            {
                Projectile.ai[1]++;
                if (Projectile.ai[1] == 1)
                {
                    Vector2 Velocity = new Vector2(0, 25).RotatedByRandom(MathHelper.ToRadians(5));
                    Projectile.velocity = Velocity;
                }

                bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, Projectile.position, Projectile.width, Projectile.height);
                if (HasLineOfSight)
                {
                    Projectile.ai[2]++;
                }
                if (Projectile.ai[2] >= 5)
                {
                    Projectile.tileCollide = true;
                }
            }
        }

        public bool IsColliding()
        {
            int minTilePosX = (int)(Projectile.position.X / 16) - 1;
            int maxTilePosX = (int)((Projectile.position.X + Projectile.width) / 16) + 2;
            int minTilePosY = (int)(Projectile.position.Y / 16) - 1;
            int maxTilePosY = (int)((Projectile.position.Y + Projectile.height) / 16) + 2;
            if (minTilePosX < 0)
            {
                minTilePosX = 0;
            }
            if (maxTilePosX > Main.maxTilesX)
            {
                maxTilePosX = Main.maxTilesX;
            }
            if (minTilePosY < 0)
            {
                minTilePosY = 0;
            }
            if (maxTilePosY > Main.maxTilesY)
            {
                maxTilePosY = Main.maxTilesY;
            }

            for (int i = minTilePosX; i < maxTilePosX; ++i)
            {
                for (int j = minTilePosY; j < maxTilePosY; ++j)
                {
                    if (Main.tile[i, j] != null && (Main.tile[i, j].HasTile && (Main.tileSolid[(int)Main.tile[i, j].TileType])))
                    {
                        Vector2 vector2;
                        vector2.X = (float)(i * 16);
                        vector2.Y = (float)(j * 16);

                        if (Projectile.position.X + Projectile.width > vector2.X && Projectile.position.X < vector2.X + 16.0 && 
                        (Projectile.position.Y + Projectile.height > (double)vector2.Y && Projectile.position.Y < vector2.Y + 16.0))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void OnKill(int timeLeft)
		{
            if (Projectile.ai[0] > 0)
            {
                SoundEngine.PlaySound(SoundID.Item89 with { Volume = 0.5f }, Projectile.Center);

                float maxAmount = 30;
                int currentAmount = 0;
                while (currentAmount <= maxAmount)
                {
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(1f, 10f), Main.rand.NextFloat(1f, 10f));
                    Vector2 Bounds = new Vector2(Main.rand.NextFloat(1f, 6f), Main.rand.NextFloat(1f, 6f));
                    float intensity = Main.rand.NextFloat(1f, 10f);

                    Vector2 vector12 = Vector2.UnitX * 0f;
                    vector12 += -Vector2.UnitY.RotatedBy((double)(currentAmount * (6f / maxAmount)), default) * Bounds;
                    vector12 = vector12.RotatedBy(velocity.ToRotation(), default);
                    int num104 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Torch, 0f, 0f, 100, default, 3f);
                    Main.dust[num104].noGravity = true;
                    Main.dust[num104].position = Projectile.Center + vector12;
                    Main.dust[num104].velocity = velocity * 0f + vector12.SafeNormalize(Vector2.UnitY) * intensity;
                    currentAmount++;
                }
            }
		}
    }
}