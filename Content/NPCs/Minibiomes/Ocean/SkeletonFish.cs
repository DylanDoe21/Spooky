using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Ocean
{
    public class SkeletonFish : ModNPC
    {
		Vector2 SavePosition = Vector2.Zero;

        private static Asset<Texture2D> NPCTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;
            NPCID.Sets.CountsAsCritter[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 80;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.width = 60;
            NPC.height = 36;
            NPC.npcSlots = 0.5f;
            NPC.knockBackResist = 0.35f;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = false;
            NPC.value = Item.buyPrice(0, 0, 0, 25);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.DeathSound = SoundID.DD2_SkeletonHurt;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ZombieOceanBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.SkeletonFish"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ZombieOceanBiome>().ModBiomeBestiaryInfoElement)
            });
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            NPCTexture ??= ModContent.Request<Texture2D>(Texture);

            //draw aura
            Vector2 drawOrigin = new(NPCTexture.Width() * 0.5f, NPC.height * 0.5f);

            var effects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            //draw aura
            for (int i = 0; i < 360; i += 30)
            {
                Color color = new Color(125 - NPC.alpha, 125 - NPC.alpha, 125 - NPC.alpha, 0).MultiplyRGBA(Color.Lime);

                Vector2 circular = new Vector2(2.5f, 0).RotatedBy(MathHelper.ToRadians(i));

                spriteBatch.Draw(NPCTexture.Value, NPC.Center + circular - screenPos, NPC.frame, color, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            spriteBatch.Draw(NPCTexture.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, effects, 0f);

            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 10 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 4)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void AI()
        {
			if (NPC.velocity.X > 0f)
			{
				NPC.direction = 1;
			}
			if (NPC.velocity.X < 0f)
			{
				NPC.direction = -1;
			}

			NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.05f : -0.05f);

			if (SavePosition == Vector2.Zero)
			{
				NPC.ai[0] = Main.rand.Next(-400, 400);
				SavePosition = NPC.Center;
			}

			FishSwimmingAI(NPC, SavePosition, 150, 120, 4f, 2.35f, 0.018f, 0.05f);
        }

		public static void FishSwimmingAI(NPC NPC, Vector2 GoTo, int MaxDistX, int MaxDistY, float VelocityCapX, float VelocityCapY, float VelocityAccel, float OutOfAreaAccel)
		{
			//handle moving away from big dunk if he swims near, to give the fish a sense of fear
			int BigDunk = NPC.FindFirstNPC(ModContent.NPCType<Dunkleosteus>());
			if (BigDunk >= 0)
			{
				bool DunkLineOfSight = Collision.CanHitLine(Main.npc[BigDunk].position, Main.npc[BigDunk].width, Main.npc[BigDunk].height, NPC.position, NPC.width, NPC.height);
				if (NPC.Distance(Main.npc[BigDunk].Center) <= 200f && DunkLineOfSight && NPC.wet)
				{
					NPC.ai[1] = 2;

					float Aceleration = 0.1f;
                    if (NPC.Center.X < Main.npc[BigDunk].Center.X)
                    {
                        NPC.velocity.X -= Aceleration;
                    }
                    else
                    {
                        NPC.velocity.X += Aceleration;
                    }
                    if (NPC.Center.Y < Main.npc[BigDunk].Center.Y)
                    {
                        NPC.velocity.Y -= Aceleration;
                    }
                    else
                    {
                        NPC.velocity.Y += Aceleration;
                    }
				}
			}

			//NPC.ai[2] is used to check for fish fleeing behavior
			if (NPC.ai[1] <= 0)
			{
				NPC.ai[0]++;

				//if inside the x-bounds, then swim around
				if (NPC.Center.X > GoTo.X - MaxDistX && NPC.Center.X < GoTo.X + MaxDistX)
				{
					//swim right
					if (NPC.ai[0] > -200f && NPC.ai[0] < 200f)
					{
						if (NPC.velocity.X < VelocityCapX)
						{
							NPC.velocity.X += VelocityAccel;
						}
					}
					//swim left
					else
					{
						if (NPC.velocity.X > -VelocityCapX)
						{
							NPC.velocity.X -= VelocityAccel;
						}
					}
				}
				//if not, then accelerate to go back in
				else
				{
					if (NPC.Center.X <= GoTo.X - MaxDistX && NPC.velocity.X < VelocityCapX)
					{
						NPC.velocity.X += OutOfAreaAccel;
					}
					if (NPC.Center.X >= GoTo.X + MaxDistX && NPC.velocity.X > -VelocityCapX)
					{
						NPC.velocity.X -= OutOfAreaAccel;
					}
				}

				//if inside the y-bounds, then swim around
				if (NPC.Center.Y > GoTo.Y - MaxDistY && NPC.Center.Y < GoTo.Y + (MaxDistY / 2))
				{
					//swim down
					if (NPC.ai[0] > 0f)
					{
						if (NPC.velocity.Y < VelocityCapY)
						{
							NPC.velocity.Y += VelocityAccel;
						}
					}
					//swim up
					else
					{
						if (NPC.velocity.Y > -VelocityCapY)
						{
							NPC.velocity.Y -= VelocityAccel;
						}
					}
				}
				//if not, then accelerate to go back in
				else
				{
					if (NPC.Center.Y <= GoTo.Y - MaxDistY && NPC.velocity.Y < VelocityCapY)
					{
						NPC.velocity.Y += OutOfAreaAccel;
					}
					if (NPC.Center.Y >= GoTo.Y + (MaxDistY / 2) && NPC.velocity.Y > -VelocityCapY)
					{
						NPC.velocity.Y -= OutOfAreaAccel;
					}
				}

				if (NPC.ai[0] > 400f)
				{
					NPC.ai[0] = -400f;
				}

				float CollideSpeed = 0.7f;
				if (NPC.collideX)
				{
					NPC.netUpdate = true;
					NPC.velocity.X = NPC.oldVelocity.X * (0f - CollideSpeed);
					if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
					{
						NPC.velocity.X = 2f;
					}
					if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
					{
						NPC.velocity.X = -2f;
					}
				}
				if (NPC.collideY)
				{
					NPC.netUpdate = true;
					NPC.velocity.Y = NPC.oldVelocity.Y * (0f - CollideSpeed);
					if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
					{
						NPC.velocity.Y = 2f;
					}
					if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
					{
						NPC.velocity.Y = -2f;
					}
				}

				if (!NPC.wet)
				{
					if (NPC.velocity.Y < 0f)
					{
						NPC.velocity.Y *= 0.95f;
					}

					NPC.velocity.Y += 0.5f;
					if (NPC.velocity.Y > 5f)
					{
						NPC.velocity.Y = 5f;
					}
				}
			}
			else
			{
				NPC.ai[1]--;
			}
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/SkeletonFishGore" + numGores).Type);
                    }
                }
            }
        }
    }
}