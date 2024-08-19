using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.Projectiles.Catacomb
{
    public class DaffodilHandMinion : ModProjectile
    {
        bool isAttacking = false;

        private static Asset<Texture2D> ChainTexture;
        private static Asset<Texture2D> ProjTexture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 30;
            Projectile.height = 32;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.player[Projectile.owner];

            ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Catacomb/DaffodilHandMinionChain");
			
			Rectangle? chainSourceRectangle = null;

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
			Vector2 chainDrawPosition = new Vector2(Projectile.Center.X + (Projectile.spriteDirection == -1 ? -3 : 3), Projectile.Center.Y + 5);
			Vector2 vectorFromProjectileToPlayer = player.Center.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayer = vectorFromProjectileToPlayer.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height());

			if (chainSegmentLength == 0)
			{
				chainSegmentLength = 10;
			}

			float chainRotation = unitVectorFromProjectileToPlayer.ToRotation() + MathHelper.PiOver2;
			int chainCount = 0;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayer.Length() + chainSegmentLength / 2f;

			while (chainLengthRemainingToDraw > 0f)
			{
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				chainDrawPosition += unitVectorFromProjectileToPlayer * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

            if (isAttacking)
            {
                ProjTexture ??= ModContent.Request<Texture2D>(Texture);

                Vector2 drawOrigin = new(ProjTexture.Width() * 0.5f, Projectile.height * 0.5f);

                for (int numEffect = 0; numEffect < 4; numEffect++)
                {
                    var effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                    Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y) + (numEffect / 4 * 6f + Projectile.rotation + 0f).ToRotationVector2() - Main.screenPosition + new Vector2(0, Projectile.gfxOffY) - Projectile.velocity * numEffect;
                    Color color = new Color(127 - Projectile.alpha, 127 - Projectile.alpha, 127 - Projectile.alpha, 0).MultiplyRGBA(Color.Red);
                    Rectangle rectangle = new(0, ProjTexture.Height() / Main.projFrames[Projectile.type] * Projectile.frame, ProjTexture.Width(), ProjTexture.Height() / Main.projFrames[Projectile.type]);
                    Main.EntitySpriteDraw(ProjTexture.Value, vector, rectangle, color, Projectile.rotation, drawOrigin, Projectile.scale * 1.2f, effects, 0);
                }
            }

            return true;
        }

        public override bool? CanDamage()
        {
			return isAttacking;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.ai[0] = 9;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.spriteDirection = Projectile.Center.X > player.Center.X ? 1 : -1;

            if (player.dead)
			{
				player.GetModPlayer<SpookyPlayer>().DaffodilHand = false;
			}

			if (player.GetModPlayer<SpookyPlayer>().DaffodilHand)
			{
				Projectile.timeLeft = 2;
			}

            Vector2 vector = new Vector2(player.Center.X, player.Center.Y);
            float RotateX = Projectile.Center.X - vector.X;
            float RotateY = Projectile.Center.Y - vector.Y;
            Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

            //target an enemy
            for (int i = 0; i < 200; i++)
            {
                NPC Target = Projectile.OwnerMinionAttackTargetNPC;
                if (Target != null && Target.CanBeChasedBy(this) && !NPCID.Sets.CountsAsCritter[Target.type] && Vector2.Distance(player.Center, Target.Center) <= 300f)
                {
                    AttackingAI(Target, player);

                    break;
                }
                else
                {
                    isAttacking = false;
                }

                NPC NPC = Main.npc[i];
                if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(player.Center, NPC.Center) <= 300f)
                {
                    AttackingAI(NPC, player);

                    break;
                }
                else
                {
                    isAttacking = false;
                }
            }

            if (!isAttacking || Vector2.Distance(player.Center, Projectile.Center) > 350f)
            {
                IdleAI(player);
            }

            //prevent Projectiles clumping together
            for (int k = 0; k < Main.projectile.Length; k++)
            {
                Projectile other = Main.projectile[k];
                if (k != Projectile.whoAmI && other.type == Projectile.type && other.active && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width)
                {
                    const float pushAway = 0.5f;
                    if (Projectile.position.X < other.position.X)
                    {
                        Projectile.velocity.X -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.X += pushAway;
                    }
                    if (Projectile.position.Y < other.position.Y)
                    {
                        Projectile.velocity.Y -= pushAway;
                    }
                    else
                    {
                        Projectile.velocity.Y += pushAway;
                    }
                }
            }
		}

        public void AttackingAI(NPC target, Player player)
		{
            isAttacking = true;

            Projectile.frame = 1;

            Projectile.ai[0]++;

            //punch enemies constantly while they are nearby
            if (Projectile.ai[0] == 1)
            {
                Vector2 PunchSpeed = target.Center - Projectile.Center;
                PunchSpeed.Normalize(); 
                PunchSpeed *= 20;
                Projectile.velocity = PunchSpeed;
            }

            if (Projectile.ai[0] == 10)
            {
                Vector2 ReturnSpeed = player.Center - Projectile.Center;
                ReturnSpeed.Normalize(); 
                ReturnSpeed *= 20;
                Projectile.velocity = ReturnSpeed;
            }

            //loop ai
            if (Projectile.ai[0] >= 15 || Projectile.Hitbox.Intersects(player.Hitbox))
            {
                Projectile.ai[0] = 0;
            }
        }

        public void IdleAI(Player player)
		{
            Projectile.frame = 0;

            Projectile.ai[0] = 0;

            if (Vector2.Distance(new Vector2(player.Center.X, player.Center.Y - 50), Projectile.Center) > 50f)
            {
                Vector2 GoTo = player.Center;
                GoTo.X += player.direction == -1 ? 10 : -10;
                GoTo.Y -= 80;

                float vel = MathHelper.Clamp(Projectile.Distance(GoTo) / 12, 8, 15);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(GoTo) * vel, 0.08f);

                if (Vector2.Distance(new Vector2(player.Center.X, player.Center.Y - 50), Projectile.Center) > 450f)
            {
                    Projectile.position = player.Center;
                }
            }
            else
            {
                Projectile.velocity *= 0.8f;
            }
        }
    }
}