using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Content.Items.SpookyHell.Sentient;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientChumCasterBobber : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 20;
            Projectile.aiStyle = ProjAIStyleID.Bobber;
            Projectile.bobber = true;
            Projectile.penetrate = -1;
        }

        public override bool PreDrawExtras()
        {
            Player player = Main.player[Projectile.owner];
            Item item = player.HeldItem;

            int xPositionAdditive = 45;
            float yPositionAdditive = 35f;

            if (!Projectile.bobber || item.holdStyle <= 0)
                return false;

            Texture2D fishingLineTexture = TextureAssets.FishingLine.Value;
            float originX = player.MountedCenter.X;
            float originY = player.MountedCenter.Y;
            originY += player.gfxOffY;
            //This variable is used to account for Gravitation Potions
            float gravity = player.gravDir;

            if (item.type == ModContent.ItemType<SentientChumCaster>())
            {
                originX += (float)(xPositionAdditive * player.direction);
                if (player.direction < 0)
                {
                    originX -= 13f;
                }
                originY -= yPositionAdditive * gravity;
            }

            if (gravity == -1f)
            {
                originY -= 12f;
            }
            Vector2 mountedCenter = new Vector2(originX, originY);
            mountedCenter = player.RotatedRelativePoint(mountedCenter + new Vector2(8f), true) - new Vector2(8f);
            Vector2 lineOrigin = Projectile.Center - mountedCenter;
            bool canDraw = true;
            if (lineOrigin.X == 0f && lineOrigin.Y == 0f)
                return false;

            float projPosMagnitude = lineOrigin.Length();
            projPosMagnitude = 12f / projPosMagnitude;
            lineOrigin.X *= projPosMagnitude;
            lineOrigin.Y *= projPosMagnitude;
            mountedCenter -= lineOrigin;
            lineOrigin = Projectile.Center - mountedCenter;

            while (canDraw)
            {
                float height = 12f;
                float positionMagnitude = lineOrigin.Length();
                if (float.IsNaN(positionMagnitude) || float.IsNaN(positionMagnitude))
                    break;

                if (positionMagnitude < 20f)
                {
                    height = positionMagnitude - 8f;
                    canDraw = false;
                }
                positionMagnitude = 12f / positionMagnitude;
                lineOrigin.X *= positionMagnitude;
                lineOrigin.Y *= positionMagnitude;
                mountedCenter += lineOrigin;
                lineOrigin = Projectile.Center - mountedCenter;
                if (positionMagnitude > 12f)
                {
                    float positionInverseMultiplier = 0.3f;
                    float absVelocitySum = Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y);
                    if (absVelocitySum > 16f)
                    {
                        absVelocitySum = 16f;
                    }
                    absVelocitySum = 1f - absVelocitySum / 16f;
                    positionInverseMultiplier *= absVelocitySum;
                    absVelocitySum = positionMagnitude / 80f;
                    if (absVelocitySum > 1f)
                    {
                        absVelocitySum = 1f;
                    }
                    positionInverseMultiplier *= absVelocitySum;
                    if (positionInverseMultiplier < 0f)
                    {
                        positionInverseMultiplier = 0f;
                    }
                    absVelocitySum = 1f - Projectile.localAI[0] / 100f;
                    positionInverseMultiplier *= absVelocitySum;
                    if (lineOrigin.Y > 0f)
                    {
                        lineOrigin.Y *= 1f + positionInverseMultiplier;
                        lineOrigin.X *= 1f - positionInverseMultiplier;
                    }
                    else
                    {
                        absVelocitySum = Math.Abs(Projectile.velocity.X) / 3f;
                        if (absVelocitySum > 1f)
                        {
                            absVelocitySum = 1f;
                        }
                        absVelocitySum -= 0.5f;
                        positionInverseMultiplier *= absVelocitySum;
                        if (positionInverseMultiplier > 0f)
                        {
                            positionInverseMultiplier *= 2f;
                        }
                        lineOrigin.Y *= 1f + positionInverseMultiplier;
                        lineOrigin.X *= 1f - positionInverseMultiplier;
                    }
                }

                Color lineColor = Lighting.GetColor((int)mountedCenter.X / 16, (int)mountedCenter.Y / 16, Color.Red);

                //This color decides the color of the fishing line.
                if (Projectile.ai[2] == 1)
                {
                    lineColor = Lighting.GetColor((int)mountedCenter.X / 16, (int)mountedCenter.Y / 16, Color.Blue);
                }

                float rotation = lineOrigin.ToRotation() - MathHelper.PiOver2;

                Main.spriteBatch.Draw(fishingLineTexture, new Vector2(mountedCenter.X - Main.screenPosition.X + fishingLineTexture.Width * 0.5f, mountedCenter.Y - Main.screenPosition.Y + fishingLineTexture.Height * 0.5f), new Rectangle(0, 0, fishingLineTexture.Width, (int)height), lineColor, rotation, new Vector2(fishingLineTexture.Width * 0.5f, 0f), 1f, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
