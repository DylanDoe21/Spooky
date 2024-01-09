using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;

using Spooky.Content.Buffs.Pets;

namespace Spooky.Content.Projectiles.Pets
{
	public class TrapdoorSpiderMount : ModMount
	{
		bool ShouldUseFloatMovement = false;

		public static readonly SoundStyle WalkSound = new("Spooky/Content/Sounds/SpiderMountWalk", SoundType.Sound);

		protected class SpiderLegData
		{
			internal int NumLegs;

			internal Vector2[] AnchorPoint;
			internal Vector2[] LegPosition;
			internal Vector2[] LegPositionDistance;
			internal Vector2 MountCenter = new Vector2(0f, 2f);

			//frame height for the leg texture (half the size of the sprite since the leg texture has 2 frames)
			internal float LegFrameHeight = 58f;

			public SpiderLegData()
			{
				NumLegs = 4;
				AnchorPoint = new Vector2[NumLegs];
				LegPosition = new Vector2[NumLegs];
				LegPositionDistance = new Vector2[NumLegs];
			}
		}

		public override void SetStaticDefaults()
		{
			MountData.constantJump = false;
			MountData.jumpSpeed = 15f;
			MountData.jumpHeight = 0;
			MountData.flightTimeMax = 0;
			MountData.runSpeed = 6.8f;
			MountData.acceleration = 0.75f;
			MountData.fallDamage = 0f;
			MountData.totalFrames = 1;
			MountData.bodyFrame = 3;
			MountData.heightBoost = 65;
			MountData.fatigueMax = 320;
			MountData.yOffset = 0;
            MountData.playerHeadOffset = 65;
			MountData.usesHover = false;
			MountData.idleFrameLoop = true;
            MountData.constantJump = false;
            MountData.playerYOffsets = Enumerable.Repeat(50, MountData.totalFrames).ToArray();
			MountData.buff = ModContent.BuffType<TrapdoorSpiderMountBuff>();

			if (!Main.dedServ)
			{
				MountData.textureWidth = Utils.Width(MountData.backTexture);
				MountData.textureHeight = Utils.Height(MountData.backTexture);
			}
		}

		public override void SetMount(Player player, ref bool mounted)
		{
			player.mount._mountSpecificData = new SpiderLegData();
			SpiderLegData SpiderLegData = (SpiderLegData)player.mount._mountSpecificData;
			for (int i = 0; i < SpiderLegData.NumLegs; i++)
			{
				SpiderLegData.AnchorPoint[i] = player.Center;
				SpiderLegData.LegPosition[i] = player.Center;
			}
		}

		public override void UpdateEffects(Player player)
		{
			SpiderLegData SpiderLegData = (SpiderLegData)player.mount._mountSpecificData;
			float standingHeight = 0f;
			int grounded = 0;
			for (int i = 0; i < SpiderLegData.NumLegs; i++)
			{
				Vector2 LegOffsets = new Vector2(((float)(i % 4) - ((float)SpiderLegData.NumLegs / 2f - 0.5f)) * 100f, 0f);

				if (i >= 8)
				{
					LegOffsets.X += (float)player.direction * 48f;
				}

				float MoveDelay = 15f;
				int MovingTimer = (int)(((float)Main.GameUpdateCount + MoveDelay / 4f * (float)i) % MoveDelay);

				Vector2 LegDestination = FindAirBlock(LegOffsets * 0.4f, player.Center + SpiderLegData.MountCenter + player.velocity * 10f);
				Vector2 LegDestinationAir = FindAirBlock(LegOffsets * 0.3f, player.Center + SpiderLegData.MountCenter + new Vector2((player.direction == -1 ? -5f : 5f), 75f));

				if ((Vector2.Distance(SpiderLegData.AnchorPoint[i], LegDestination) > SpiderLegData.LegFrameHeight || Vector2.Distance(SpiderLegData.AnchorPoint[i], player.Center + SpiderLegData.MountCenter) > SpiderLegData.LegFrameHeight) && MovingTimer == 1)
				{
					SpiderLegData.AnchorPoint[i] = FindAnchorTile(LegDestination, 25);
				}
				if (!Collision.SolidCollision(SpiderLegData.AnchorPoint[i], 25, 25, true) && !Collision.IsWorldPointSolid(SpiderLegData.AnchorPoint[i], false))
				{
					SpiderLegData.AnchorPoint[i] = FindAnchorTile(LegDestinationAir, 25);
				}
				if (Collision.SolidCollision(SpiderLegData.AnchorPoint[i], 1, 1, true) || Collision.IsWorldPointSolid(SpiderLegData.AnchorPoint[i], false))
				{
					grounded++;
				}

				float legCorrectionSpeed = 0.6f;
				SpiderLegData.LegPosition[i].X = MathHelper.Lerp(SpiderLegData.LegPosition[i].X, SpiderLegData.AnchorPoint[i].X, legCorrectionSpeed);
				SpiderLegData.LegPosition[i].Y = MathHelper.Lerp(SpiderLegData.LegPosition[i].Y, SpiderLegData.AnchorPoint[i].Y, legCorrectionSpeed);
				SpiderLegData.LegPositionDistance[i] = new Vector2(0f, Vector2.Distance(SpiderLegData.LegPosition[i], SpiderLegData.AnchorPoint[i]) * -0.8f);
				standingHeight += SpiderLegData.AnchorPoint[i].Y - player.Center.Y;
			}

			if (player.velocity.Y > 0)
			{
				player.controlUp = false;
			}

			standingHeight *= 1f / (float)SpiderLegData.NumLegs;
			if (standingHeight > SpiderLegData.LegFrameHeight * 0.5f && !player.controlJump && !player.controlUp)
			{
				player.mount._fatigue = 320f;
			}
			else
			{
				player.mount._fatigue = 0f;
			}
			if (grounded >= 4)
			{
				player.gravity = 0;
				player.mount._flyTime = 2;
				MountData.flightTimeMax = 2;
				MountData.usesHover = true;

				if (player.controlDown)
				{
					player.velocity.Y = 3f;
				}
			}
			else
			{
				player.mount._flyTime = 0;
				MountData.flightTimeMax = 0;
				MountData.usesHover = false;
			}
		}

		public Vector2 FindAnchorTile(Vector2 AnchorPosition, int range)
		{
			Vector2 AnchorDestination = Vector2.Zero;
			Vector2 AnchorPoint = Vector2.Zero;

			for (int i = 0; i < range * 5; i++)
			{
				float angle = (float)(360 / 5) * (float)(i % 5);
				float dist = (float)Math.Floor((float)i * 0.25f);
				AnchorDestination = AnchorPosition + Utils.RotatedBy(new Vector2(0f, dist) * 4f, (double)angle, default(Vector2));

				if (Collision.IsWorldPointSolid(AnchorDestination, false))
				{
					AnchorPoint = AnchorDestination;
					break;
				}

				if (i == range - 1)
				{
					AnchorPoint = AnchorPosition + new Vector2(0f, 0f);
				}
			}

			return AnchorPoint;
		}

		public Vector2 FindAirBlock(Vector2 pos, Vector2 origin)
		{
			float amount = 1f;
			for (int i = 0; i < 100; i++)
			{
				if (!Collision.SolidCollision(pos + origin, 15, 15, true))
				{
					break;
				}
				amount += 1f;
				pos /= amount;
			}

			return pos + origin;
		}

		public float Cosine(float length, Player player)
		{
			SpiderLegData newSpiderObject = (SpiderLegData)player.mount._mountSpecificData;
			float Variable1 = newSpiderObject.LegFrameHeight;
			float Variable2 = newSpiderObject.LegFrameHeight;
			return (float)Math.Acos(((float)Math.Pow(Variable1, 2.0) + (float)Math.Pow(Variable2, 2.0) - (float)Math.Pow(length, 2.0)) / (2f * Variable1 * Variable2));
		}

		public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawplayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
		{
			SpiderLegData SpiderLegData = (SpiderLegData)drawplayer.mount._mountSpecificData;

			//drawType 0 = draws behind the mount and player
			if (drawType == 0)
			{
				for (int i = 0; i < SpiderLegData.NumLegs; i++)
				{
					Vector2 legEnd = SpiderLegData.LegPosition[i] + SpiderLegData.LegPositionDistance[i];
					float drawRotation = 0f;
					float angle = Cosine(Math.Min(Vector2.Distance(legEnd, drawplayer.Center + SpiderLegData.MountCenter), SpiderLegData.LegFrameHeight * 1.99f), drawplayer);
					float angleOther = (MathHelper.ToRadians(180f) - angle) * 0.5f;
					float angleParent = Utils.ToRotation(legEnd - (drawplayer.Center + SpiderLegData.MountCenter)) + MathHelper.ToRadians(-90f);

					SpriteEffects effects = SpriteEffects.None;
					if (SpiderLegData.LegPosition[i].X > drawplayer.Center.X)
					{
						angleOther *= -1f;
						effects = SpriteEffects.FlipHorizontally;
					}

					float Angle1 = angleOther + angleParent;
					float Angle2 = 0f - angleOther + angleParent;

					for (int j = 0; j < 2; j++)
					{
						Texture2D LegTexture = ModContent.Request<Texture2D>("Spooky/Content/Projectiles/Pets/TrapdoorSpiderMountLeg", AssetRequestMode.ImmediateLoad).Value;
						Vector2 position = legEnd + drawPosition - drawplayer.Center;
						int legSegments = 2;
                        Rectangle rect = new Rectangle(0, LegTexture.Height / legSegments * j, LegTexture.Width, LegTexture.Height / legSegments);
                        Vector2 origin = new Vector2((float)LegTexture.Width * 0.5f, 0f);

						if (j == 0)
						{
							drawRotation = Angle1;
							position = drawPosition + SpiderLegData.MountCenter;
						}
						else
						{
							drawRotation = Angle2;
							position = Utils.RotatedBy(new Vector2(0f, SpiderLegData.LegFrameHeight), (double)Angle1, default(Vector2)) + drawPosition + SpiderLegData.MountCenter;
						}

                        DrawData drawData = new DrawData(LegTexture, position, (Rectangle?)rect, drawColor, drawRotation, origin, drawScale, effects, -1f);
						int currentShader = drawplayer.cMount;
						drawData.shader = currentShader;
						playerDrawData.Add(drawData);
					}
				}
			}

			//draw and rotate the mount a little based on the players x-velocity
			if (drawType == 0 || drawType == 1)
			{
				rotation = drawplayer.velocity.X * 0.01f;
			}

			//do not draw the front texture
			if (drawType == 2)
			{
				return false;
			}
			
			return true;
		}
	}
}
