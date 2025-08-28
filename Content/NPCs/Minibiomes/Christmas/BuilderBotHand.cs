using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
	public class BuilderBotHand : ModNPC
	{
		private static Asset<Texture2D> ChainTexture;
		private static Asset<Texture2D> NPCTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5000;
			NPC.damage = 35;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 18;
			NPC.npcSlots = 0f;
			NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.noTileCollide = true;
			NPC.noGravity = true;
			NPC.dontCountMe = true;
			NPC.hide = true;
			NPC.aiStyle = -1;
		}

		public void DrawArms(bool SpawnGore, Color EyeDrawColor)
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			//only draw if the parent is active
			if (Parent.active && Parent.type == ModContent.NPCType<BuilderBot>() && !SpawnGore)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Minibiomes/Christmas/BuilderBotHandChain");

				Vector2 ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == 1 ? -5 : 5), Parent.Center.Y + 5);
				Vector2 NpcOrigin = new Vector2(NPC.Center.X, NPC.Center.Y);

				if (NPC.type == ModContent.NPCType<BuilderBotHandBack>())
				{
					ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == -1 ? -5 : 5), Parent.Center.Y + 5);
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NpcOrigin;
				Vector2 p0 = ParentOrigin;
				Vector2 p1 = ParentOrigin;
				Vector2 p2 = myCenter;
				Vector2 p3 = myCenter;

				int Length = (int)NPC.Distance(ParentOrigin) / 12;
				int max = Length <= 10 ? 7 : Length;

				if (NPC.type == ModContent.NPCType<BuilderBotHandBack>())
				{
					max = Length <= 10 ? 12 : Length;
				}

				int segments = max;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = (drawPosNext - drawPos2);
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					Color GetlightColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));
					Color BuffTintColor = Parent.GetNPCColorTintedByBuffs(GetlightColor);

					Main.EntitySpriteDraw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, BuffTintColor, rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			if (SpawnGore)
			{
				Vector2 ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == 1 ? -5 : 5), Parent.Center.Y + 5);
				Vector2 NpcOrigin = new Vector2(NPC.Center.X, NPC.Center.Y);

				if (NPC.type == ModContent.NPCType<BuilderBotHandBack>())
				{
					ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == -1 ? -5 : 5), Parent.Center.Y + 5);
				}

				Vector2 myCenter = NpcOrigin;
				Vector2 p0 = ParentOrigin;
				Vector2 p1 = ParentOrigin;
				Vector2 p2 = myCenter;
				Vector2 p3 = myCenter;

				int Length = (int)NPC.Distance(ParentOrigin) / 12;
				int max = Length <= 10 ? 7 : Length;

				if (NPC.type == ModContent.NPCType<BuilderBotHandBack>())
				{
					max = Length <= 10 ? 12 : Length;
				}

				int segments = max;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);

					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/BuilderBotHandChainGore").Type);
					}
				}
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawArms(false, drawColor);

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			float RotateX = Parent.Center.X - NPC.Center.X;
            float RotateY = Parent.Center.Y - NPC.Center.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			//kill the hand if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<BuilderBot>())
			{
				DrawArms(true, Color.White);

				if (Main.netMode != NetmodeID.Server) 
                {
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BuilderBotHandGore").Type);
				}

				NPC.active = false;
			}

			bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
			
			if (HasLineOfSight && player.Distance(Parent.Center) <= 350f)
			{
				Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 5;
            	NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
				NPC.noTileCollide = false;
			}
			else
			{
				GoToPosition(Parent.spriteDirection == 1 ? 20 : -20, 5, 1f);
				NPC.noTileCollide = true;
			}
		}

		public void GoToPosition(float X, float Y, float speed)
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			float goToX = (Parent.Center.X + X);
			float goToY = (Parent.Center.Y + Y);

			//slow down when close enough to the parent npc
			if (NPC.Distance(new Vector2(goToX, goToY)) <= 5f)
			{
				NPC.velocity *= 0.95f;
			}
			else
			{
				Vector2 desiredVelocity = NPC.DirectionTo(new Vector2(goToX, goToY)) * 10;
            	NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
			}
		}
	}

	public class BuilderBotHandBack : BuilderBotHand
	{
		public override string Texture => "Spooky/Content/NPCs/Minibiomes/Christmas/BuilderBotHand";

		private static Asset<Texture2D> NPCTexture;

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawArms(false, drawColor);

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            var effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0);

			return false;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCProjectiles.Add(index);
		}

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

			float RotateX = Parent.Center.X - NPC.Center.X;
            float RotateY = Parent.Center.Y - NPC.Center.Y;
            NPC.rotation = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;

			//kill the hand if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<BuilderBot>())
			{
				DrawArms(true, Color.White);

				if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BuilderBotHandGore").Type);
                }

                NPC.active = false;
			}

			bool HasLineOfSight = Collision.CanHitLine(player.position, player.width, player.height, NPC.position, NPC.width, NPC.height);
			
			if (HasLineOfSight && player.Distance(Parent.Center) <= 350f)
			{
				Vector2 desiredVelocity = NPC.DirectionTo(player.Center) * 5;
            	NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 1f / 20);
				NPC.noTileCollide = false;
			}
			else
			{
				GoToPosition(Parent.spriteDirection == 1 ? 35 : -35, 5, 1f);
				NPC.noTileCollide = true;
			}
		}
	}
}