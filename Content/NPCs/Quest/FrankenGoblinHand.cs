using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

using Spooky.Core;

namespace Spooky.Content.NPCs.Quest
{
	public class FrankenGoblinHandFront : ModNPC
	{
		public override string Texture => "Spooky/Content/NPCs/Quest/FrankenGoblinHand";

		float EyeScale = 0f;

		Vector2 SavePlayerPosition;

		private static Asset<Texture2D> ChainTexture;
		private static Asset<Texture2D> EyeHoldTexture;

		public override void SetStaticDefaults()
		{
			NPCID.Sets.CantTakeLunchMoney[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			//vector2
			writer.WriteVector2(SavePlayerPosition);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			//vector2
			SavePlayerPosition = reader.ReadVector2();
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5000;
			NPC.damage = 35;
			NPC.defense = 0;
			NPC.width = 44;
			NPC.height = 44;
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
			if (Parent.active && Parent.type == ModContent.NPCType<FrankenGoblin>() && !SpawnGore)
			{
				ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/FrankenGoblinArm");

				Vector2 ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == 1 ? -25 : 25), Parent.Center.Y + (Parent.localAI[3] == 0 ? 45 : 20));
				Vector2 NpcOrigin = new Vector2(NPC.Center.X, NPC.Center.Y);

				if (NPC.type == ModContent.NPCType<FrankenGoblinHandBack>())
				{
					ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == -1 ? -45 : 45), Parent.Center.Y + (Parent.localAI[3] == 0 ? 45 : 20));
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NpcOrigin;
				Vector2 p0 = ParentOrigin;
				Vector2 p1 = ParentOrigin;
				Vector2 p2 = myCenter;
				Vector2 p3 = myCenter;

				int Length = (int)NPC.Distance(ParentOrigin) / 12;
				int max = Length <= 10 ? 7 : Length;

				if (NPC.type == ModContent.NPCType<FrankenGoblinHandBack>())
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

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, GetlightColor, rotation, drawOrigin, NPC.scale * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}

				if (Parent.localAI[3] == 4 && Parent.localAI[0] >= 80 && Parent.localAI[0] < 170)
				{
					EyeHoldTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Quest/FrankenGoblinEye");

            		Vector2 eyeDrawOrigin = new Vector2(EyeHoldTexture.Width() / 2, EyeHoldTexture.Height() / 2);
					Vector2 drawPos = new Vector2(NPC.Center.X, NPC.Center.Y - 20) - Main.screenPosition;

					if (EyeScale < 1f)
					{
						EyeScale += 0.02f;
					}

					Main.spriteBatch.Draw(EyeHoldTexture.Value, drawPos, null, EyeDrawColor, 0, eyeDrawOrigin, EyeScale, SpriteEffects.None, 0f);
				}
				else
				{
					EyeScale = 0f;
				}
			}

			if (SpawnGore)
			{
				Vector2 ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == 1 ? -25 : 25), Parent.Center.Y + (Parent.localAI[3] == 0 ? 45 : 20));
				Vector2 NpcOrigin = new Vector2(NPC.Center.X, NPC.Center.Y);

				if (NPC.type == ModContent.NPCType<FrankenGoblinHandBack>())
				{
					ParentOrigin = new Vector2(Parent.Center.X + (Parent.spriteDirection == -1 ? -45 : 45), Parent.Center.Y + (Parent.localAI[3] == 0 ? 45 : 20));
				}

				Vector2 myCenter = NpcOrigin;
				Vector2 p0 = ParentOrigin;
				Vector2 p1 = ParentOrigin;
				Vector2 p2 = myCenter;
				Vector2 p3 = myCenter;

				int Length = (int)NPC.Distance(ParentOrigin) / 12;
				int max = Length <= 10 ? 7 : Length;

				if (NPC.type == ModContent.NPCType<FrankenGoblinHandBack>())
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
						Gore.NewGore(NPC.GetSource_Death(), drawPos2, NPC.velocity, ModContent.Find<ModGore>("Spooky/FrankenGoblinArmGore").Type);
					}
				}
			}
		}
		
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			DrawArms(false, drawColor);

			return true;
		}

		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCProjectiles.Add(index);
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			return Parent.localAI[3] == 2;
		}

		public override bool CheckActive()
        {
            return false;
        }

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			Player player = Main.player[Parent.target];

			NPC.spriteDirection = Parent.spriteDirection;

			//kill the hand if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<FrankenGoblin>())
			{
				DrawArms(true, Color.White);

				if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FrankenGoblinHandGore").Type);
                }

                NPC.active = false;
			}

			NPC.noGravity = Parent.localAI[3] != 0;
			NPC.noTileCollide = Parent.localAI[3] != 0;

			switch ((int)Parent.localAI[3])
			{
				case 1:
				{
					NPC.rotation = 0;

					GoToPosition(Parent.spriteDirection == 1 ? -75 : 75, 35, 0.45f);

					break;
				}

				case 2:
				{
					if (Parent.localAI[0] >= 120 && Parent.localAI[0] < 480 && Parent.velocity.Y == 0)
					{
						NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.1f;

						float goToX = player.Center.X - NPC.Center.X;
						float goToY = player.Center.Y - NPC.Center.Y;
						float speed = 0.065f;

						if (NPC.velocity.X < goToX)
						{
							NPC.velocity.X = NPC.velocity.X + speed;
							if (NPC.velocity.X < 0f && goToX > 0f)
							{
								NPC.velocity.X = NPC.velocity.X + speed * 3;
							}
						}
						else if (NPC.velocity.X > goToX)
						{
							NPC.velocity.X = NPC.velocity.X - speed;
							if (NPC.velocity.X > 0f && goToX < 0f)
							{
								NPC.velocity.X = NPC.velocity.X - speed * 3;
							}
						}
						if (NPC.velocity.Y < goToY)
						{
							NPC.velocity.Y = NPC.velocity.Y + speed;
							if (NPC.velocity.Y < 0f && goToY > 0f)
							{
								NPC.velocity.Y = NPC.velocity.Y + speed;
								return;
							}
						}
						else if (NPC.velocity.Y > goToY)
						{
							NPC.velocity.Y = NPC.velocity.Y - speed;
							if (NPC.velocity.Y > 0f && goToY < 0f)
							{
								NPC.velocity.Y = NPC.velocity.Y - speed;
								return;
							}
						}
					}
					else
					{
						NPC.rotation = 0;

						GoToPosition(Parent.spriteDirection == 1 ? -75 : 75, 35, 0.45f);
					}

					break;
				}

				case 3:
				{
					NPC.rotation = 0;

					GoToPosition(Parent.spriteDirection == 1 ? -75 : 75, 35, Parent.localAI[0] < 60 ? 0.45f : 2.65f);

					break;
				}

				case 4:
				{
					if (Parent.localAI[0] >= 80 && Parent.localAI[0] < 200)
					{
						NPC.rotation = 3.14f;
					}
					else
					{
						NPC.rotation = 0;
					}

					//go above parent a bit
					if (Parent.localAI[0] < 80 || Parent.localAI[0] >= 200)
					{
						GoToPosition(Parent.spriteDirection == 1 ? -75 : 75, 35, 0.45f);
					}

					//right before throwing, move down
					if (Parent.localAI[0] >= 80 && Parent.localAI[0] < 140)
					{
						GoToPosition(Parent.spriteDirection == 1 ? -75 : 75, 65, 0.45f);
					}
		
					//when throwing eyes, move up
					if (Parent.localAI[0] >= 140 && Parent.localAI[0] < 200)
					{
						if (Parent.localAI[0] == 170)
						{
							int Eye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, ((int)NPC.Center.Y - 25) + NPC.height / 2, ModContent.NPCType<FrankenGoblinEye>());
							Main.npc[Eye].velocity = NPC.velocity * 1.5f;
						}

						GoToPosition(Parent.spriteDirection == 1 ? -75 : 75, -200, 0.75f);
					}

					break;
				}
			}
		}

		public void GoToPosition(float X, float Y, float speed)
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			float goToX = (Parent.Center.X + X) - NPC.Center.X;
			float goToY = (Parent.Center.Y + Y) - NPC.Center.Y;

			NPC.ai[1]++;

			if (NPC.velocity.X > speed)
			{
				NPC.velocity.X *= 0.98f;
			}
			if (NPC.velocity.Y > speed)
			{
				NPC.velocity.Y *= 0.98f;
			}

			//slow down when close enough to the parent npc
			if (NPC.Distance(Parent.Center) <= 150f)
			{
				NPC.velocity *= 0.92f;
			}

			if (NPC.velocity.X < goToX)
			{
				NPC.velocity.X = NPC.velocity.X + speed;
				if (NPC.velocity.X < 0f && goToX > 0f)
				{
					NPC.velocity.X = NPC.velocity.X + speed;
				}
			}
			else if (NPC.velocity.X > goToX)
			{
				NPC.velocity.X = NPC.velocity.X - speed;
				if (NPC.velocity.X > 0f && goToX < 0f)
				{
					NPC.velocity.X = NPC.velocity.X - speed;
				}
			}
			if (NPC.velocity.Y < goToY)
			{
				NPC.velocity.Y = NPC.velocity.Y + speed;
				if (NPC.velocity.Y < 0f && goToY > 0f)
				{
					NPC.velocity.Y = NPC.velocity.Y + speed;
					return;
				}
			}
			else if (NPC.velocity.Y > goToY)
			{
				NPC.velocity.Y = NPC.velocity.Y - speed;
				if (NPC.velocity.Y > 0f && goToY < 0f)
				{
					NPC.velocity.Y = NPC.velocity.Y - speed;
					return;
				}
			}
		}
	}

	public class FrankenGoblinHandBack : FrankenGoblinHandFront
	{
		public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsBehindNonSolidTiles.Add(index);
		}

		public override void AI()
		{
			NPC Parent = Main.npc[(int)NPC.ai[0]];

			Player player = Main.player[Parent.target];

			NPC.spriteDirection = -Parent.spriteDirection;

			//kill the hand if the parent does not exist
			if (!Parent.active || Parent.type != ModContent.NPCType<FrankenGoblin>())
			{
				if (Main.netMode != NetmodeID.Server) 
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/FrankenGoblinHandGore").Type);
                }

                NPC.active = false;
			}

			NPC.noGravity = Parent.localAI[3] != 0;
			NPC.noTileCollide = Parent.localAI[3] != 0;

			switch ((int)Parent.localAI[3])
			{
				case 1:
				{
					NPC.rotation = 0;

					GoToPosition(Parent.spriteDirection == -1 ? -85 : 85, 35, 0.45f);

					break;
				}

				case 2:
				{
					if (Parent.localAI[0] >= 120 && Parent.localAI[0] < 480 && Parent.velocity.Y == 0)
					{
						NPC.rotation += (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) * 0.1f;

						float goToX = player.Center.X - NPC.Center.X;
						float goToY = player.Center.Y - NPC.Center.Y;
						float speed = 0.065f;

						if (NPC.velocity.X < goToX)
						{
							NPC.velocity.X = NPC.velocity.X + speed;
							if (NPC.velocity.X < 0f && goToX > 0f)
							{
								NPC.velocity.X = NPC.velocity.X + speed * 3;
							}
						}
						else if (NPC.velocity.X > goToX)
						{
							NPC.velocity.X = NPC.velocity.X - speed;
							if (NPC.velocity.X > 0f && goToX < 0f)
							{
								NPC.velocity.X = NPC.velocity.X - speed * 3;
							}
						}
						if (NPC.velocity.Y < goToY)
						{
							NPC.velocity.Y = NPC.velocity.Y + speed;
							if (NPC.velocity.Y < 0f && goToY > 0f)
							{
								NPC.velocity.Y = NPC.velocity.Y + speed;
								return;
							}
						}
						else if (NPC.velocity.Y > goToY)
						{
							NPC.velocity.Y = NPC.velocity.Y - speed;
							if (NPC.velocity.Y > 0f && goToY < 0f)
							{
								NPC.velocity.Y = NPC.velocity.Y - speed;
								return;
							}
						}
					}
					else
					{
						NPC.rotation = 0;

						GoToPosition(Parent.spriteDirection == -1 ? -85 : 85, 35, 0.45f);
					}

					break;
				}

				case 3:
				{
					NPC.rotation = 0;

					GoToPosition(Parent.spriteDirection == -1 ? -85 : 85, 35, Parent.localAI[0] < 60 ? 0.45f : 2.65f);

					break;
				}

				case 4:
				{
					if (Parent.localAI[0] >= 80 && Parent.localAI[0] < 200)
					{
						NPC.rotation = 3.14f;
					}
					else
					{
						NPC.rotation = 0;
					}

					//go above parent a bit
					if (Parent.localAI[0] < 80 || Parent.localAI[0] >= 200)
					{
						GoToPosition(Parent.spriteDirection == -1 ? -85 : 85, 35, 0.45f);
					}

					//right before throwing, move down
					if (Parent.localAI[0] >= 80 && Parent.localAI[0] < 140)
					{
						GoToPosition(Parent.spriteDirection == -1 ? -85 : 85, 65, 0.45f);
					}
		
					//when throwing eyes, move up
					if (Parent.localAI[0] >= 140 && Parent.localAI[0] < 200)
					{
						if (Parent.localAI[0] == 170)
						{
							int Eye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, ((int)NPC.Center.Y - 25) + NPC.height / 2, ModContent.NPCType<FrankenGoblinEye>());
							Main.npc[Eye].velocity = NPC.velocity * 1.5f;
						}

						GoToPosition(Parent.spriteDirection == -1 ? -85 : 85, -200, 0.75f);
					}

					break;
				}
			}
		}
	}
}