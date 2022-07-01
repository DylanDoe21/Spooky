using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class HalloweenSlime : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halloween Slime");
            Main.npcFrameCount[NPC.type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 25;
            NPC.damage = 10;
            NPC.width = 38;
            NPC.height = 26;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("A small and squishy pumpkin that can be seen hopping around in the spooky forest. No one knows if it is a pumpkin or a slime. Or maybe both?")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn on the surface during the day
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && Main.dayTime)
                {
                    return 20f;
                }
            }

            return 0f;
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
        }
		
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter >= 6)
            {
                NPC.frame.Y = (NPC.frame.Y + frameHeight) % (Main.npcFrameCount[NPC.type] * frameHeight);
                NPC.frameCounter = 1;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Gel, 0, 1, 2));
        }

        public override bool CheckDead() 
		{
            for (int numDust = 0; numDust < 20; numDust++)
            {
                int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), NPC.width / 2, NPC.height / 2, 288, 0f, 0f, 100, default, 2f);

                Main.dust[DustGore].velocity *= 3f;
                Main.dust[DustGore].noGravity = true;

                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[DustGore].scale = 0.5f;
                    Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            return true;
        }
    }
}