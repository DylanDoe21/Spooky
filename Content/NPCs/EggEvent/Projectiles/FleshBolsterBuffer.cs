using Terraria;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Buffs;

namespace Spooky.Content.NPCs.EggEvent.Projectiles
{
	public class FleshBolsterBuffer : ModProjectile
	{
		public static List<int> BuffableNPCs = new List<int>()
		{
			ModContent.NPCType<CoughLungs>(),
			ModContent.NPCType<CruxBat>(),
			ModContent.NPCType<ExplodingAppendix>(),
			ModContent.NPCType<GooSlug>(),
			ModContent.NPCType<HoppingHeart>(),
			ModContent.NPCType<HoverBrain>(),
			ModContent.NPCType<TongueBiter>()
		};

		int target = 0;

		private static Asset<Texture2D> ChainTexture;
		private static Asset<Texture2D> ChainTexture1;
		private static Asset<Texture2D> ChainTexture2;
		private static Asset<Texture2D> ChainTexture3;

		public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.tileCollide = false;
			Projectile.hide = true;
			Projectile.timeLeft = 2;
			Projectile.penetrate = -1;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCs.Add(index);
		}

		//TODO: make this chain better visually, maybe more like a tube rather than just segments
		public override bool PreDraw(ref Color lightColor)
		{
			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			//only draw if the parent is active
			if (Parent.active && Parent.type == ModContent.NPCType<FleshBolster>())
			{
				ChainTexture1 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/FleshBolsterBufferSegment1");
				ChainTexture2 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/FleshBolsterBufferSegment2");
				ChainTexture3 ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/EggEvent/Projectiles/FleshBolsterBufferSegment3");

				Vector2 ParentCenter = Parent.Center;

				Rectangle? chainSourceRectangle = null;
				float chainHeightAdjustment = 0f;

				Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (ChainTexture1.Size() / 2f);
				Vector2 chainDrawPosition = new Vector2(Projectile.Center.X, Projectile.Center.Y);
				Vector2 vectorToParent = ParentCenter.MoveTowards(chainDrawPosition, 4f) - chainDrawPosition;
				Vector2 unitVectorToParent = vectorToParent.SafeNormalize(Vector2.Zero);
				float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : ChainTexture1.Height()) + chainHeightAdjustment;

				if (chainSegmentLength == 0)
				{
					chainSegmentLength = 10;
				}

				float chainRotation = unitVectorToParent.ToRotation() + MathHelper.PiOver2;
				int chainCount = 0;
				float chainLengthRemainingToDraw = vectorToParent.Length() + chainSegmentLength / 2f;

				while (chainLengthRemainingToDraw > 0f)
				{
					Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

					if (chainCount == 1)
					{
						Main.spriteBatch.Draw(ChainTexture1.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
					}
					else if (chainCount == 2)
					{
						Main.spriteBatch.Draw(ChainTexture2.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
					}
					else if (chainCount > 2)
					{
						Main.spriteBatch.Draw(ChainTexture3.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);
					}

					chainDrawPosition += unitVectorToParent * chainSegmentLength;
					chainCount++;
					chainLengthRemainingToDraw -= chainSegmentLength;
				}
			}

			if (target != 0)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/Projectiles/TrailSquare");

				Vector2 TargetPosition = Main.npc[target].Center;

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

					Main.spriteBatch.Draw(ChainTexture.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, Color.DeepPink, chainRotation, chainOrigin, 3f * fade, SpriteEffects.None, 0f);

					chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
					chainCount++;
					chainLengthRemainingToDraw -= chainSegmentLength;
				}
			}

			return true;
		}

		public override bool? CanDamage()
		{
			return false;
		}

		public override void AI()
		{
			Player player = Main.player[Projectile.owner];

			NPC Parent = Main.npc[(int)Projectile.ai[2]];

			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				Projectile.frame++;
				if (Projectile.frame >= 6)
				{
					Projectile.frame = 0;
				}
			}

			Projectile.timeLeft = 2;

			//die if the parent npc is dead
			if (!Parent.active || Parent.type != ModContent.NPCType<FleshBolster>())
			{
				Projectile.active = false;
			}

			Vector2 vector = new Vector2(Projectile.Center.X, Projectile.Center.Y);
			float RotateX = Parent.Center.X - vector.X;
			float RotateY = Parent.Center.Y - vector.Y;
			Projectile.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			Vector2 home = Parent.Center + new Vector2(75, 0).RotatedBy(MathHelper.ToRadians(60) * Projectile.ai[1]);
			Vector2 distance = home - Projectile.Center;
			float range = distance.Length();
			distance.Normalize();

			distance /= Main.rand.Next(4, 9);

			Projectile.velocity += distance;

			if (range > 30f)
			{
				Projectile.velocity *= 0.96f;
			}

			if (target == 0)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];
					if (npc.CanBeChasedBy(Projectile) && BuffableNPCs.Contains(npc.type))
					{
						if (npc.Distance(Parent.Center) <= 550f && !npc.GetGlobalNPC<NPCGlobal>().BeingBuffedByBolster)
						{
							target = i;
							Main.npc[target].GetGlobalNPC<NPCGlobal>().BeingBuffedByBolster = true;
						}
					}
				}
			}
			else
			{
				NPC Target = Main.npc[target];

				Target.AddBuff(ModContent.BuffType<EggEventEnemyBuff>(), 10);

				//stop buffing the enemy if it gets to far away from the bolster
				if (!Target.active || Target.Distance(Parent.Center) > 600f)
				{
					Target.GetGlobalNPC<NPCGlobal>().BeingBuffedByBolster = false;
					target = 0;
				}
			}
		}
	}
}