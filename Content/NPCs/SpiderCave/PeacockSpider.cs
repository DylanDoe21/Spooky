using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class PeacockSpider1 : ModNPC
	{
        Vector2 SavePosition;

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			SavePosition = reader.ReadVector2();
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 80;
            NPC.damage = 22;
			NPC.defense = 5;
			NPC.width = 50;
			NPC.height = 70;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit33;
			NPC.DeathSound = SoundID.NPCDeath16;
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PeacockSpider1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.ai[0] <= 1)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            else
            {
                if (NPC.ai[1] < 60)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 5 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 4)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else
                {
                    if (NPC.ai[2] <= 10)
                    {
                        //squish frame before jumping up
                        NPC.frame.Y = 4 * frameHeight;
                    }
                    else
                    {
                        //jumping up frame
                        if (NPC.velocity.Y < 0)
                        {
                            NPC.frame.Y = 5 * frameHeight;
                        }
                        //falling frame
                        else if (NPC.velocity.Y > 0)
                        {
                            NPC.frame.Y = 1 * frameHeight;
                        }
                        //idle frame
                        else
                        {
                            NPC.frame.Y = 3 * frameHeight;
                        }
                    }
                }
            }
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            switch ((int)NPC.ai[0])
            {
                case 0:
                {
                    SavePosition = NPC.Center;

                    NPC.ai[0]++;

                    break;
                }
                case 1:
                {
                    NPC.rotation = 0;
                    NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;

                    MoveBackAndFourth(SavePosition, 2f, 0.025f, 150, true);

                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                    if (NPC.Distance(player.Center) <= 200f && lineOfSight)
                    {
                        NPC.ai[0]++;
                    }

                    break;
                }
                case 2:
                {
                    NPC.rotation = NPC.velocity.Y * 0.05f;
                    NPC.spriteDirection = NPC.direction;

                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);
                    if (!lineOfSight)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                    }
                    
                    if (NPC.velocity.Y == 0 || NPC.ai[1] > 60)
                    {
                        NPC.ai[1]++;
                    }
                    if (NPC.ai[1] < 60)
                    {
                        MoveBackAndFourth(player.Center, 4f, 0.5f, 60, false);
                    }
                    if (NPC.ai[1] == 60)
                    {
                        NPC.velocity.X = 0;
                    }
                    if (NPC.ai[1] > 60)
                    {
                        //set where the it should be jumping towards
                        Vector2 JumpTo = new(player.Center.X, NPC.Center.Y - 500);

                        //set velocity and speed
                        Vector2 velocity = JumpTo - NPC.Center;
                        velocity.Normalize();

                        float speed = MathHelper.Clamp(velocity.Length() / 36, 12, 22);

                        //actual jumping
                        if (NPCGlobalHelper.IsCollidingWithFloor(NPC))
                        {
                            NPC.ai[2]++;
                            if (NPC.ai[2] == 10)
                            {
                                velocity.Y -= 0.25f;
                            }
                        }

                        if (NPC.ai[2] > 10 && NPC.velocity.X == 0)
                        {
                            velocity.X *= 1.2f;
                            NPC.velocity = velocity * speed;
                        }
                    }

                    if (NPC.ai[2] > 10 && NPC.velocity.Y > 0 && NPC.collideY)
                    {
                        NPC.velocity.X = 0;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                    }

                    break;
                }
            }
        }

        public void MoveBackAndFourth(Vector2 Center, float MaxSpeed, float Acceleration, int Distance, bool ResetCheck)
        {
            //prevents the pet from getting stuck on sloped tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            Vector2 center2 = NPC.Center;
            Vector2 vector48 = Center - center2;
            float CenterDistance = vector48.Length();

            if (CenterDistance > Distance * 2 && NPC.velocity.Y == 0 && ResetCheck)
            {
                NPC.ai[0] = 0;
            }

            if (NPC.velocity.Y == 0 && HoleBelow() && CenterDistance > 100f)
            {
                NPC.velocity.Y = -10f;
                NPC.netUpdate = true;
            }

            if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
            {
                if (NPC.velocity.Y == 0 && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 65) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                {
                    NPC.velocity.Y = -10f;
                    NPC.netUpdate = true;
                }
            }

            if (NPC.collideX)
            {
                NPC.velocity.X = -NPC.velocity.X;
            }

            NPC.velocity.Y += 0.15f;

            if (NPC.velocity.Y > 10f)
            {
                NPC.velocity.Y = 10f;
            }

            if (CenterDistance > Distance)
            {
                if (Center.X - NPC.position.X > 0f)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
            else
            {
                if (NPC.velocity.X >= 0)
                {
                    NPC.velocity.X += Acceleration;
                    if (NPC.velocity.X > MaxSpeed)
                    {
                        NPC.velocity.X = MaxSpeed;
                    }
                }
                else
                {
                    NPC.velocity.X -= Acceleration;
                    if (NPC.velocity.X < -MaxSpeed)
                    {
                        NPC.velocity.X = -MaxSpeed;
                    }
                }
            }
        }

        public bool HoleBelow()
        {
            int tileWidth = 4;
            int tileX = (int)(NPC.Center.X / 16f) - tileWidth;
            if (NPC.velocity.X > 0)
            {
                tileX += tileWidth;
            }
            int tileY = (int)((NPC.position.Y + NPC.height) / 16f);
            for (int y = tileY; y < tileY + 2; y++)
            {
                for (int x = tileX; x < tileX + tileWidth; x++)
                {
                    if (Main.tile[x, y].HasTile && (Main.tile[x - 1, y].HasTile || Main.tile[x + 1, y].HasTile))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SpiderChitin>(), 3, 1, 3));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PeacockSpiderBlueGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class PeacockSpider2 : PeacockSpider1
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PeacockSpider2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PeacockSpiderPurpleGore" + numGores).Type);
                    }
                }
            }
        }
    }

    public class PeacockSpider3 : PeacockSpider1
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PeacockSpider3"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PeacockSpiderPinkGore" + numGores).Type);
                    }
                }
            }
        }
    }
}