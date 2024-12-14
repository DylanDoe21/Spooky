using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs.Debuff;

namespace Spooky.Content.Projectiles.SpookyHell
{
    public class GlassEye : ModProjectile
    {   
		bool isAttacking = false;

		NPC EnemyToDrawTo = null;

		private static Asset<Texture2D> ChainTexture;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }
        
        public override void SetDefaults()
        {
			Projectile.width = 16;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
			Projectile.netImportant = true;
			Projectile.hide = true;
			Projectile.timeLeft = 2;
            Projectile.penetrate = -1;
        }

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			if (EnemyToDrawTo != null)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

				Vector2 TargetPosition = EnemyToDrawTo.Center;

				Rectangle? chainSourceRectangle = null;

				float chainHeightAdjustment = 0f;

				Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture.Size() / 2f);
				Vector2 chainDrawPosition = Projectile.Center;
				Vector2 vectorFromProjectileToPlayerArms = TargetPosition.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
				Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
				float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture.Height()) + chainHeightAdjustment;

				if (chainSegmentLength == 0)
				{
					chainSegmentLength = 10;
				}

				float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
				int chainCount = 0;
				float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

				while (chainLengthRemainingToDraw > 0f)
				{
					Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

					float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) / 2f + 0.5f * (float)Math.Sin(chainLengthRemainingToDraw);

					Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, Color.White, chainRotation, chainOrigin, Projectile.scale * fade, SpriteEffects.None, 0f);

					chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
					chainCount++;
					chainLengthRemainingToDraw -= chainSegmentLength;
				}
			}

			return true;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			return false;
		}

		public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

			if (player.dead)
            {
				player.GetModPlayer<SpookyPlayer>().MagicEyeOrb = false;
            }

			if (player.GetModPlayer<SpookyPlayer>().MagicEyeOrb)
            {
				Projectile.timeLeft = 2;
            }

			Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

			//movement stuff
			if (!Collision.CanHitLine(Projectile.Center, 1, 1, player.Center, 1, 1))
			{
				Projectile.ai[0] = 1f;
			}

			float speed = 8f;
			if (Projectile.ai[0] == 1f)
			{
				speed = 15f;
			}

			Vector2 center = Projectile.Center;
			Vector2 direction = player.Center - center;
			Projectile.netUpdate = true;
			int num = 1;
			for (int k = 0; k < Projectile.whoAmI; k++)
			{
				if (Main.projectile[k].active && Main.projectile[k].owner == Projectile.owner && Main.projectile[k].type == Projectile.type)
				{
					num++;
				}
			}

			direction.Y -= 70f;
			float distanceTo = direction.Length();
			if (distanceTo > 200f && speed < 9f)
			{
				speed = 9f;
			}
			if (distanceTo < 150f && Projectile.ai[0] == 1f && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Projectile.ai[0] = 0f;
				Projectile.netUpdate = true;
			}
			if (distanceTo > 2000f)
			{
				Projectile.Center = player.Center;
			}
			if (distanceTo > 100f)
			{
				direction.Normalize();
				direction *= speed;
				float temp = 40 / 2f;
				Projectile.velocity = (Projectile.velocity * temp + direction) / (temp + 1);
			}
			else
			{
				Projectile.velocity *= (float)Math.Pow(0.9, 40.0 / 40);
			}

			//prioritize bosses over normal enemies
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC NPC = Main.npc[i];

				if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && NPC.IsTechnicallyBoss() && Vector2.Distance(Projectile.Center, NPC.Center) <= 450f)
				{
					AttackingAI(NPC);
					return;
				}
			}

			//target an enemy
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC NPC = Main.npc[i];

				//if no boss is found, target other enemies normally
				if (NPC.active && NPC.CanBeChasedBy(this) && !NPC.friendly && !NPC.dontTakeDamage && !NPC.IsTechnicallyBoss() && !NPCID.Sets.CountsAsCritter[NPC.type] && Vector2.Distance(Projectile.Center, NPC.Center) <= 450f)
				{
					AttackingAI(NPC);
					return;
				}
				else
				{
					isAttacking = false;
				}
			}

			if (!isAttacking)
			{
				IdleAI(player);
				EnemyToDrawTo = null;
			}
		}

		public void AttackingAI(NPC target)
        {
			isAttacking = true;

			EnemyToDrawTo = target;

			Projectile.velocity *= 0.95f;

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			float RotateX = target.Center.X - vector.X;
			float RotateY = target.Center.Y - vector.Y;
			Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			target.AddBuff(ModContent.BuffType<GlassEyeDebuff>(), 2);
		}

		public void IdleAI(Player player)
        {
			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			float RotateX = player.Center.X - vector.X;
			float RotateY = player.Center.Y - vector.Y;
			Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
		}
    }
}