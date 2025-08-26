using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
    public class BuilderBot : ModNPC  
    {
        bool SpawnedHands = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                CustomTexturePath = "Spooky/Content/NPCs/NPCDisplayTextures/BuilderBotBestiary"
            };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //bools
            writer.Write(SpawnedHands);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //bools
            SpawnedHands = reader.ReadBoolean();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 75;
            NPC.damage = 30;
            NPC.defense = 10;
            NPC.width = 36;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit4;
			NPC.DeathSound = SoundID.Item14;
            NPC.hide = true;
            NPC.aiStyle = 3;
            AIType = NPCID.GoblinScout;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BuilderBot"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.ChristmasDungeonBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 5)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 6)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void DrawBehind(int index)
		{
			Main.instance.DrawCacheNPCsOverPlayers.Add(index);
		}
        
        public override void AI()
		{
            NPC.spriteDirection = NPC.direction;

            if (!SpawnedHands)
			{
				int FrontHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-45, -20), (int)NPC.Center.Y, ModContent.NPCType<BuilderBotHand>(), ai0: NPC.whoAmI);
				int BackHand = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(20, 35), (int)NPC.Center.Y, ModContent.NPCType<BuilderBotHandBack>(), ai0: NPC.whoAmI);

				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					NetMessage.SendData(MessageID.SyncNPC, number: FrontHand);
					NetMessage.SendData(MessageID.SyncNPC, number: BackHand);
				}

				SpawnedHands = true;
				NPC.netUpdate = true;
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
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BuilderBotGore" + numGores).Type);
                    }
                }
            }
        }
    }
}