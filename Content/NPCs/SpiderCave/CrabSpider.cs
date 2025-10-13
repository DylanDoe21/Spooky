using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class CrabSpider1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 9;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 25;
			NPC.defense = 12;
			NPC.width = 46;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath17 with { Pitch = 0.5f };
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CrabSpider1"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
        {
            //default walking
            if (NPC.localAI[0] == 0)
            {
                if (NPC.velocity.X != 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 8)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 7)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                else
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //walking backwards animation
            else if (NPC.localAI[0] == 1)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5 - (NPC.velocity.X > 0 ? NPC.velocity.X / 2 : -(NPC.velocity.X / 2)))
                {
                    NPC.frame.Y = NPC.frame.Y - frameHeight;
                    NPC.frameCounter = 0;
                }

                if (NPC.frame.Y <= frameHeight * 0)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
            //jumping attack frames
            else if (NPC.localAI[0] == 2)
            {
                //jumping up frame
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
                //falling frame
                else if (NPC.velocity.Y >= 0)
                {
                    NPC.frameCounter++;
                    if (NPC.frameCounter > 6)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0;
                    }
                    if (NPC.frame.Y >= frameHeight * 7)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                }
                //squish frame before jumping up
                else if (NPC.ai[0] <= 20 && NPC.velocity.Y == 0)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            switch ((int)NPC.localAI[0])
            {
                case 0:
                {
                    NPC.aiStyle = 66;
                    AIType = NPCID.Worm;

                    if (NPC.Distance(player.Center) <= 200f)
                    {
                        NPC.ai[0] = 0;
                        NPC.localAI[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }
                case 1:
                {
                    NPC.aiStyle = -1;

                    NPC.rotation = 0;
                    NPC.spriteDirection = NPC.direction = NPC.velocity.X >= 0 ? -1 : 1;

                    bool lineOfSight = Collision.CanHitLine(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height);

                    //prevents the pet from getting stuck on sloped tiles
                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                    Vector2 center2 = NPC.Center;
                    Vector2 vector48 = player.Center - center2;
                    float CenterDistance = vector48.Length();

                    if (NPC.velocity.Y == 0 && HoleBelow() && CenterDistance > 100f)
                    {
                        NPC.velocity.Y = -15f;
                        NPC.netUpdate = true;
                    }

                    if ((NPC.velocity.X < 0f && NPC.direction == 1) || (NPC.velocity.X > 0f && NPC.direction == -1))
                    {
                        if (NPC.velocity.Y == 0 && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * -50) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                        {
                            NPC.velocity.Y = -15f;
                            NPC.netUpdate = true;
                        }
                    }

                    NPC.velocity.Y += 0.35f;

                    if (NPC.velocity.Y > 15f)
                    {
                        NPC.velocity.Y = 15f;
                    }

                    if (player.Center.X - NPC.position.X > 0f)
                    {
                        NPC.velocity.X -= lineOfSight ? 0.1f : -0.1f;
                        if (NPC.velocity.X > 2f)
                        {
                            NPC.velocity.X = 2f;
                        }
                    }
                    else
                    {
                        NPC.velocity.X += lineOfSight ? 0.1f : -0.1f;
                        if (NPC.velocity.X < -2f)
                        {
                            NPC.velocity.X = -2f;
                        }
                    }

                    if (NPC.Distance(player.Center) >= 500f)
                    {
                        NPC.localAI[0]++;
                        NPC.netUpdate = true;
                    }

                    break;
                }
                case 2:
                {
                    NPC.aiStyle = 66;
			        AIType = NPCID.Worm;

                    NPC.rotation = NPC.velocity.Y * 0.025f;
                
                    NPC.ai[0]++;
                    if (NPC.ai[0] < 20)
                    {
                        NPC.velocity.X *= 0.975f;
                        NPC.velocity.Y += 0.02f;
                        if (NPC.velocity.Y > 8f)
                        {
                            NPC.velocity.Y = 8f;
                        }
                    }
                    if (NPC.ai[0] == 20)
                    {
                        Vector2 ChargeDirection = new Vector2(player.Center.X, NPC.Center.Y - (NPC.Distance(player.Center) / 2)) - NPC.Center;
                        ChargeDirection.Normalize();
                                
                        ChargeDirection *= 15;
                        NPC.velocity.X = ChargeDirection.X;
                        NPC.velocity.Y = ChargeDirection.Y;

                        NPC.ai[1]++;
                    }

                    //loop ai
                    if (NPC.ai[0] >= 40 && NPC.collideY)
                    {
                        NPC.ai[0] = 0;
                        if (NPC.ai[1] >= 2 || NPC.Distance(player.Center) <= 50f)
                        {
                            NPC.ai[1] = 0;
                            NPC.localAI[0] = 1;
                        }

                        NPC.netUpdate = true;
                    }

                    break;
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
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CrabSpiderBrownGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class CrabSpider2 : CrabSpider1
	{
        public override void SetDefaults()
		{
            NPC.lifeMax = 100;
            NPC.damage = 25;
			NPC.defense = 12;
			NPC.width = 46;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.1f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath17 with { Pitch = 0.5f };
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpiderCaveBiome>().Type };
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CrabSpider2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpiderCaveBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CrabSpiderWhiteGore" + numGores).Type);
                    }
                }
            }
        }
    }
}