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
        private static Asset<Texture2D> ProjTexture;
        private static Asset<Texture2D> ChainTexture;

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 44;
            Projectile.DamageType = DamageClass.Generic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ProjTexture ??= ModContent.Request<Texture2D>(Texture);
            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Cemetery/SlendermanTentacleSegment");

            Player player = Main.player[Projectile.owner];

			if (!player.dead)
			{
                bool flip = false;
                if (player.direction == -1 || Projectile.Center.Y < player.Center.Y)
                {
                    flip = true;
                }

                Vector2 drawProjOrigin = new Vector2(0, ProjTexture.Height() / 2);
                Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = Projectile.Center;
				Vector2 p0 = player.Center;
				Vector2 p1 = player.Center;
				Vector2 p2 = myCenter;
				Vector2 p3 = myCenter;

                int segments = 12;

                for (int i = 0; i < segments; i++)
                {
                    float t = i / (float)segments;
                    Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                    t = (i + 1) / (float)segments;
                    Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
                    Vector2 toNext = (drawPosNext - drawPos2);
                    float rotation = toNext.ToRotation();
                    float distance = toNext.Length();

                    lightColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

                    Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(lightColor), 
                    rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
                }
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

            Vector2 ProjectileVelocity = player.position - player.oldPosition;
            Projectile.position += ProjectileVelocity;
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

                if (range > 120f)
                {
                    Projectile.ai[0] = -1f;
                    Projectile.netUpdate = true;
                }

                Projectile.velocity += distance;
                
                if (range > 30f)
                {
                    Projectile.velocity *= 0.96f;
                }

                if (Projectile.ai[0] > 120f)
                {
                    Projectile.ai[0] = 10 + Main.rand.Next(10);

                    float maxDistance = 400f;
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
                        Projectile.velocity -= ProjectileVelocity;
                        Projectile.ai[0] *= -1f;
                    }

                    Projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(40) && Main.player[Projectile.owner].GetModPlayer<SpookyPlayer>().SlendermanPageDelay <= 0)
            {
                int itemType = ModContent.ItemType<SlendermanBuffPage>();
                int newItem = Item.NewItem(target.GetSource_OnHit(target), target.Hitbox, itemType);
                Main.item[newItem].noGrabDelay = 0;
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
