using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Minibiomes.Vegetable
{
    public class CornMage1 : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 300;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 36;
			NPC.height = 76;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.VegetableBiome>().Type };
        }

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement>
			{
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.CornMage1"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.VegetableBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            //walking animation
            NPC.frameCounter++;
            if (NPC.localAI[0] <= 420)
            {
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 1 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //casting animation
            if (NPC.localAI[0] > 420)
            {
                NPC.frame.Y = 7 * frameHeight;
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 420)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }

            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = 0;

                if (NPC.localAI[0] == 480 || NPC.localAI[0] == 500 || NPC.localAI[0] == 520)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 4.5f;
                }
            }

            if (NPC.localAI[0] >= 560)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidWarlockGore" + numGores).Type);
                    }
                }
            }
        }
    }
}