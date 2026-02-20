using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;
using Spooky.Content.Buffs.Minion;

namespace Spooky.Content.Projectiles.Sentient
{
    public class SentientOpticStaffMouth : ModProjectile
    {
        float DistFromPlayer = 75f;

        bool isAttacking = false;

        private static Asset<Texture2D> GlowTexture;
        private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 34;
            Projectile.height = 42;
            Projectile.DamageType = DamageClass.Summon;
			Projectile.minion = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.timeLeft = 2;
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
		{
			Player player = Main.player[Projectile.owner];

			ChainTexture ??= ModContent.Request<Texture2D>(Texture + "Chain");

			Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
			Vector2 myCenter = Projectile.Center;
			Vector2 p0 = player.Center;
			Vector2 p1 = player.Center;
			Vector2 p2 = myCenter;
			Vector2 p3 = myCenter;

			int segments = 5;

			for (int i = 0; i < segments; i++)
			{
				float t = i / (float)segments;
				Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				t = (i + 1) / (float)segments;
				Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
				Vector2 toNext = drawPosNext - drawPos2;
				float rotation = toNext.ToRotation();
				float distance = toNext.Length();

				Color color = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

				Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, Projectile.GetAlpha(color), rotation, drawOrigin, Projectile.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
			}

			return true;
		}

        public override void PostDraw(Color lightColor)
        {
            GlowTexture ??= ModContent.Request<Texture2D>(Texture + "Glow");

            var spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            int height = GlowTexture.Height() / Main.projFrames[Projectile.type];
            int frameHeight = height * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, frameHeight, GlowTexture.Width(), height);

            Main.EntitySpriteDraw(GlowTexture.Value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
            rectangle, Color.White, Projectile.rotation, new Vector2(GlowTexture.Width() / 2f, height / 2f), Projectile.scale, spriteEffects, 0);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (player.dead || !player.active) 
            {
				player.ClearBuff(ModContent.BuffType<SentientOpticStaffBuff>());
			}

			if (player.HasBuff(ModContent.BuffType<SentientOpticStaffBuff>()))
            {
				Projectile.timeLeft = 2;
			}

            if (Projectile.ai[0] >= 30)
            {
                Projectile.frame = 2;
            }   
            else
            {
                Projectile.frameCounter++;
                if (Projectile.frameCounter >= 6)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                    if (Projectile.frame >= 2)
                    {
                        Projectile.frame = 0;
                    }
                }
            }

            Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
            float RotateX = player.Center.X - vector.X;
            float RotateY = player.Center.Y - vector.Y;
            Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //target an enemy
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 550f)
                {
                    AttackingAI(Target);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 550f)
                {
                    AttackingAI(NPC);

                    break;
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (!isAttacking)
            {
                Projectile.ai[0] = 0;
            }

            Vector2 destination = player.MountedCenter + (MathHelper.TwoPi * Projectile.ai[1] / player.ownedProjectileCounts[Type] - MathHelper.PiOver2).ToRotationVector2() * DistFromPlayer;
            Projectile.position = destination - (Projectile.Size / 2);

            Projectile.ai[2]++;
            DistFromPlayer += (float)Math.Sin(Projectile.ai[2] / 100) / 10;
		}

        public void AttackingAI(NPC target)
		{
            isAttacking = true;

            Projectile.ai[0]++;

            //shoot cursed fireballs
            if (Projectile.ai[0] >= 60)
            {
                Vector2 ShootSpeed = target.Center - Projectile.Center;
                ShootSpeed.Normalize();
                ShootSpeed *= 18f;

                int Type = Main.rand.NextBool(5) ? ModContent.ProjectileType<SentientOpticStaffTear2>() : ModContent.ProjectileType<SentientOpticStaffTear1>();

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, ShootSpeed, Type, Projectile.damage, 2f, Projectile.owner);

                Projectile.ai[0] = 0;
            }
        }
    }
}