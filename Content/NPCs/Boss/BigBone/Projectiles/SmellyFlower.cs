using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone.Projectiles
{
	public class SmellyFlower : ModNPC
	{
		float DrawScale = 0f;

		private static Asset<Texture2D> NPCTexture;
		private static Asset<Texture2D> ChainTexture;

        public override void SetStaticDefaults()
		{
			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 5000;
            NPC.damage = 60;
            NPC.defense = 0;
            NPC.width = 60;
            NPC.height = 60;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];
			NPC BigBone = Main.npc[(int)NPC.ai[1]];

			//draw flower chain connected to big bone
			if (Parent.active && Parent.type == ModContent.NPCType<BigFlowerPot>()) 
			{
                ChainTexture ??= ModContent.Request<Texture2D>("Spooky/Content/NPCs/Boss/BigBone/Projectiles/FlowerChainNew");

                bool flip = false;
				if (Parent.direction == -1)
				{
					flip = true;
				}

				Vector2 drawOrigin = new Vector2(0, ChainTexture.Height() / 2);
				Vector2 myCenter = NPC.Center;
				Vector2 p0 = Parent.Center;
				Vector2 p1 = Parent.Center;
				Vector2 p2 = myCenter - new Vector2((NPC.alpha == 255 ? 0 : 200) * (flip ? 1 : -1), 75).RotatedBy(NPC.rotation);
				Vector2 p3 = myCenter;

				int segments = 65;

				for (int i = 0; i < segments; i++)
				{
					float t = i / (float)segments;
					Vector2 drawPos2 = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					t = (i + 1) / (float)segments;
					Vector2 drawPosNext = BezierCurveUtil.CalculateBezierPoint(t, p0, p1, p2, p3);
					Vector2 toNext = drawPosNext - drawPos2;
					float rotation = toNext.ToRotation();
					float distance = toNext.Length();

					Color ChainColor = Lighting.GetColor((int)drawPos2.X / 16, (int)(drawPos2.Y / 16));

					Main.spriteBatch.Draw(ChainTexture.Value, drawPos2 - Main.screenPosition, null, ChainColor, rotation, drawOrigin, 1f * new Vector2((distance + 4) / (float)ChainTexture.Width(), 1), SpriteEffects.None, 0f);
				}
			}

			NPCTexture ??= ModContent.Request<Texture2D>(Texture);

			if (BigBone.localAI[0] >= 90 && BigBone.localAI[0] <= 240)
			{
				float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6f)) + 0.5f;

				Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Red);

				for (int numEffect = 0; numEffect < 5; numEffect++)
				{
					Color newColor = color;
					newColor = NPC.GetAlpha(newColor);
					newColor *= 1f - fade;
					Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y) - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4);
					Main.EntitySpriteDraw(NPCTexture.Value, vector, NPC.frame, newColor, NPC.rotation, NPC.frame.Size() / 2f, DrawScale * 1.05f + fade / 3, SpriteEffects.None, 0);
				}
			}

			Main.EntitySpriteDraw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() / 2f, DrawScale, SpriteEffects.None, 0);

            return false;
        }

		public override void AI()
		{
			NPC.TargetClosest(true);
			Player player = Main.player[NPC.target];

			NPC Parent = Main.npc[(int)NPC.ai[0]];
			NPC BigBone = Main.npc[(int)NPC.ai[1]];

			if (!Parent.active || Parent.type != ModContent.NPCType<BigFlowerPot>()) 
			{
				NPC.active = false;
			}

			if (BigBone.localAI[0] < 340)
			{
				GoAboveFlowerPot(400, 0.2f);
			}
			else
			{
				if (BigBone.localAI[0] == 345)
				{
					SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);
					
					NPC.alpha = 255;

					NPC.dontTakeDamage = true;
					NPC.immortal = true;

					for (int numGores = 1; numGores <= 8; numGores++)
					{
						if (Main.netMode != NetmodeID.Server) 
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SmellyPetalGore" + Main.rand.Next(1, 4)).Type, NPC.scale);
						}
					}

					for (int numGores = 1; numGores <= 3; numGores++)
					{
						if (Main.netMode != NetmodeID.Server) 
						{
							Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SmellyGore" + numGores).Type, NPC.scale);
						}
					}
				}

				if (BigBone.localAI[0] > 350)
				{
					GoAboveFlowerPot(0, 0.75f);

					if (NPC.Hitbox.Intersects(Parent.Hitbox))
					{
						NPC.active = false;
					}
				}
			}

			if (BigBone.localAI[0] > 45 && BigBone.localAI[0] <= 150)
			{
				if (DrawScale < 1.2f)
				{
					DrawScale += 0.025f;
				}
			}

			if (BigBone.localAI[0] < 150)
			{
				NPC.rotation = MathHelper.Pi;
			}

			if (BigBone.localAI[0] > 150 && BigBone.localAI[0] <= 340)
			{
				NPC.velocity = Vector2.Zero;

				Vector2 vector = new Vector2(NPC.Center.X, NPC.Center.Y);
				float RotateX = player.Center.X - vector.X;
				float RotateY = player.Center.Y - vector.Y;

				float RotateDirection = (float)Math.Atan2((double)RotateY, (double)RotateX) + 4.71f;
				float RotateSpeed = NPC.ai[2];

				if (RotateDirection < 0f)
				{
					RotateDirection += (float)Math.PI * 2f;
				}
				if (RotateDirection > (float)Math.PI * 2f)
				{
					RotateDirection -= (float)Math.PI * 2f;
				}

				if (NPC.rotation < RotateDirection)
				{
					if ((double)(RotateDirection - NPC.rotation) > Math.PI)
					{
						NPC.rotation -= RotateSpeed;
					}
					else
					{
						NPC.rotation += RotateSpeed;
					}
				}
				if (NPC.rotation > RotateDirection)
				{
					if ((double)(NPC.rotation - RotateDirection) > Math.PI)
					{
						NPC.rotation += RotateSpeed;
					}
					else
					{
						NPC.rotation -= RotateSpeed;
					}
				}
				if (NPC.rotation > RotateDirection - RotateSpeed && NPC.rotation < RotateDirection + RotateSpeed)
				{
					NPC.rotation = RotateDirection;
				}
				if (NPC.rotation < 0f)
				{
					NPC.rotation += (float)Math.PI * 2f;
				}
				if (NPC.rotation > (float)Math.PI * 2f)
				{
					NPC.rotation -= (float)Math.PI * 2f;
				}
				if (NPC.rotation > RotateDirection - RotateSpeed && NPC.rotation < RotateDirection + RotateSpeed)
				{
					NPC.rotation = RotateDirection;
				}

				if (Main.rand.NextBool())
				{
					Vector2 pos = new Vector2(200, 0).RotatedBy(NPC.rotation + MathHelper.PiOver2) + NPC.Center;

					Vector2 ShootSpeed = (pos + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50))) - NPC.Center;
					ShootSpeed.Normalize();
					ShootSpeed *= 15f;

					SoundEngine.PlaySound(SoundID.Grass, NPC.Center);

					NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(25, 0).RotatedBy(NPC.rotation + MathHelper.PiOver2) + NPC.Center, ShootSpeed, ModContent.ProjectileType<SmellyFlowerStink>(), NPC.damage, 4.5f);
				}
			}
		}

		public void GoAboveFlowerPot(float DistanceAbove, float speed)
        {
			NPC Parent = Main.npc[(int)NPC.ai[0]];

            float goToX = Parent.Center.X - NPC.Center.X;
            float goToY = (Parent.Center.Y - DistanceAbove) - NPC.Center.Y;

            if (Vector2.Distance(NPC.Center, new Vector2(goToX, goToY)) >= 300f)
            {
				if (NPC.velocity.X > speed)
				{
					NPC.velocity.X *= 0.9f;
				}
				if (NPC.velocity.Y > speed)
				{
					NPC.velocity.Y *= 0.9f;
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
			else
			{
				NPC.velocity = Vector2.Zero;
			}
        }

		public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
				NPC BigBone = Main.npc[(int)NPC.ai[1]];

				BigBone.localAI[0] = 400;

				for (int numGores = 1; numGores <= 8; numGores++)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SmellyPetalGore" + Main.rand.Next(1, 4)).Type, NPC.scale);
					}
				}

				for (int numGores = 1; numGores <= 3; numGores++)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SmellyGore" + numGores).Type, NPC.scale);
					}
				}
			}
		}
		
        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
			var parameters = new DropOneByOne.Parameters() 
			{
				ChanceNumerator = 1,
				ChanceDenominator = 1,
				MinimumStackPerChunkBase = 1,
				MaximumStackPerChunkBase = 1,
				MinimumItemDropsCount = 1,
				MaximumItemDropsCount = 3,
			};

			npcLoot.Add(new DropOneByOne(ItemID.Heart, parameters));
        }
	}
}