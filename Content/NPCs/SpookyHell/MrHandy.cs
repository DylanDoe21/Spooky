using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class MrHandy : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mr. Handy");
            Main.npcFrameCount[NPC.type] = 3;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 56;
			NPC.height = 66;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 41;
            AIType = NPCID.Derpling;
            AnimationType = NPCID.Derpling;  
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type }; 
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("A rather energetic living hand that will aggressively hop towards it's target. It may look goofy from a distance, until it gets too close.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 30f;
            }
            return 0f;
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
        }
        
        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MrHandyGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MrHandyGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MrHandyGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MrHandyGore4").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MrHandyGore5").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/MrHandyGore6").Type);

			return true;
		}
    }
}