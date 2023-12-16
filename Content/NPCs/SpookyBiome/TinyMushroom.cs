using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class TinyMushroom : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
            Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0) 
            {
				Velocity = 1f
			};
			NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 12;
            NPC.npcSlots = 1;
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 66;
			AIType = NPCID.Buggy;
            NPC.catchItem = (short)ModContent.ItemType<TinyMushroomItem>();
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.TinyMushroom"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.velocity.X != 0)
            {
                NPC.frameCounter += 1;
                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 2)
                {
                    NPC.frame.Y = 0;
                }
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }
	}
}