using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpiderCave.Misc;
using Spooky.Content.Tiles.Blooms;

namespace Spooky.Content.NPCs.SpiderCave.SporeEvent
{
	public class BeetleMite1 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
				Velocity = 1f
			};
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.damage = 35;
            NPC.defense = 20;
			NPC.width = 40;
			NPC.height = 36;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit45 with { Pitch = -0.25f };
			NPC.DeathSound = SoundID.NPCDeath47 with { Pitch = -0.5f };
			NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SporeEventBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BeetleMite"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter > 5 - (NPC.velocity.X > 0 ? NPC.velocity.X : -NPC.velocity.X))
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }
            }
            else
            {
                NPC.frame.Y = 0 * frameHeight;
            }
		}

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (NPC.Distance(player.Center) > 250f)
            {
                NPC.spriteDirection = NPC.direction;
                NPC.velocity.X = 0;
            }
            else
            {     
                NPC.spriteDirection = NPC.direction = NPC.velocity.X <= 0 ? -1 : 1;
                MoveBackAndFourth(player.Center, 5f, 0.65f, 80);
            }
        }

        public void MoveBackAndFourth(Vector2 Center, float MaxSpeed, float Acceleration, int Distance)
        {
            //prevents the pet from getting stuck on sloped tiles
            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

            Vector2 center2 = NPC.Center;
            Vector2 vector48 = Center - center2;
            float CenterDistance = vector48.Length();

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

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MiteMandibles>(), 3, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FungusSeed>(), 120));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BeetleMiteGore" + numGores).Type);
                    }
                }
            }
        }
	}

    public class BeetleMite2 : BeetleMite1
	{
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BeetleMite2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SporeEventBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BeetleMiteEyebrowGore" + numGores).Type);
                    }
                }
            }
        }
    }
}