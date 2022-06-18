using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class Handy : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mr.Handy");
            Main.npcFrameCount[NPC.type] = 3;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 40;
            NPC.defense = 5;
            NPC.width = 68;
			NPC.height = 46;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 41;
            AIType = NPCID.Derpling;
            AnimationType = NPCID.Derpling;   
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust) && 
            ((!Main.pumpkinMoon && !Main.snowMoon) && !Main.eclipse && (SpawnCondition.GoblinArmy.Chance == 0)) && 
            Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 35f;
            }
            return 0f;
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            if (Main.rand.Next(2) == 0)
            {
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MonsterChunk>(), Main.rand.Next(1, 2)));
            }
        }
        
        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/HandyGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/HandyGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/HandyGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, ModContent.Find<ModGore>("Spooky/HandyGore4").Type);

			return true;
		}
    }
}