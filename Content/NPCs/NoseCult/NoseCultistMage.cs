using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultistMage : ModNPC
	{
		public override void SetStaticDefaults()
		{	
			Main.npcFrameCount[NPC.type] = 7;
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 300;
            NPC.damage = 40;
            NPC.defense = 15;
            NPC.width = 106;
			NPC.height = 112;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
			NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -0.4f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistMage"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

		public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 15)
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
			NPC.spriteDirection = NPC.direction;
        }
    }
}