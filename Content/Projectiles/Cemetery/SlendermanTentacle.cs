using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs;
using Spooky.Content.Items.Cemetery.Contraband;

namespace Spooky.Content.Projectiles.Cemetery
{
    public class SlendermanTentacle : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 33;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Cemetery/SlendermanTentacleSegment");

            Rectangle? chainSourceRectangle = null;
            float chainHeightAdjustment = 0f;

            Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
            Vector2 chainDrawPosition = Main.player[Projectile.owner].Center;
            Vector2 VectorToPlayer = Projectile.Center.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
            Vector2 unitVectorToPlayer = VectorToPlayer.SafeNormalize(Vector2.Zero);
            float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;

            if (chainSegmentLength == 0)
            {
                chainSegmentLength = 10;
            }

            int chainCount = 0;
            float chainLengthRemainingToDraw = VectorToPlayer.Length() + chainSegmentLength / 2f;

            while (chainLengthRemainingToDraw > 0f)
            {
                Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

                var chainTextureToDraw = chainTexture;

                Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, Projectile.rotation, chainOrigin, 1f, SpriteEffects.None, 0f);

                chainDrawPosition += unitVectorToPlayer * chainSegmentLength;
                chainCount++;
                chainLengthRemainingToDraw -= chainSegmentLength;
            }

            return true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.active && player.GetModPlayer<SpookyPlayer>().SlendermanPage)
            {
                Projectile.timeLeft = 2;
            }

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //tentacle head movement (homing)
            Vector2 playerVel = player.position - player.oldPosition;
            Projectile.position += playerVel;
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= 0f)
            {
                Vector2 home = player.Center + new Vector2(50, 0).RotatedBy(MathHelper.ToRadians(60) * Projectile.ai[1]);
                Vector2 distance = home - Projectile.Center;
                float range = distance.Length();
                distance.Normalize();
                
                if (Projectile.ai[0] == 0f)
                {
                    if (range > 13f)
                    {
                        Projectile.ai[0] = -1f; //if in fast mode, stay fast until back in range
                        if (range > 1300f)
                        {
                            Projectile.Kill();
                            return;
                        }
                    }
                    else
                    {
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 3f + Main.rand.NextFloat(-2f, 3f);
                        Projectile.netUpdate = true;
                    }
                }
                else
                {
                    distance /= 8f;
                }

                if (range > 120f) //switch to fast return mode
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }

                Projectile.velocity += distance;
                
                if (range > 30f)
                {
                    Projectile.velocity *= 0.96f;
                }

                if (Projectile.ai[0] > 120f) //attack nearby enemy
                {
                    Projectile.ai[0] = 10 + Main.rand.Next(10);
                    float maxDistance = 600f;
                    int target = -1;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.CanBeChasedBy(Projectile))
                        {
                            float npcDistance = Projectile.Distance(npc.Center);
                            if (npcDistance < maxDistance)
                            {
                                maxDistance = npcDistance;
                                target = i;
                            }
                        }
                    }
                    if (target != -1)
                    {
                        Projectile.velocity = Main.npc[target].Center - Projectile.Center;
                        Projectile.velocity.Normalize();
                        Projectile.velocity *= 13f;
                        Projectile.velocity += Main.npc[target].velocity / 2f;
                        Projectile.velocity -= playerVel;
                        Projectile.ai[0] *= -1f;
                    }

                    Projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.life <= 0)
            {
                if (Main.rand.NextBool(10) && Main.player[Projectile.owner].GetModPlayer<SpookyPlayer>().SlendermanPageDelay <= 0)
                {
                    int itemType = ModContent.ItemType<SlendermanBuffPage>();
                    int newItem = Item.NewItem(target.GetSource_OnHit(target), target.Hitbox, itemType);
                    Main.item[newItem].noGrabDelay = 0;
                }
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            //deal extra damage while the player has the slenderman page buff
            if (Main.player[Projectile.owner].HasBuff(ModContent.BuffType<SlendermanPageBuff>()))
            {
                modifiers.FlatBonusDamage += Main.rand.Next(40, 80);
            }
        }
    }
}
