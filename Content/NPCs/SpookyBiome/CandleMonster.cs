using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class CandleMonster : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Candle Monster");
            Main.npcFrameCount[NPC.type] = 4;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 220;
            NPC.damage = 55;
            NPC.defense = 10;
            NPC.width = 44;
            NPC.height = 34;
            NPC.knockBackResist = 0f;
			NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Bigger blobs of putty that take on the appearance of a pumpkin. Their ability to split into smaller versions of themselves can be really dangerous."),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiomeUg>()) && Main.hardMode)
                {
                    return 7f;
                }
            }

            return 0f;
        }

		public override void FindFrame(int frameHeight)
        {
            //use regular walking anim when in walking state
            if (NPC.localAI[0] <= 300)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

			if (NPC.localAI[0] >= 300 && NPC.localAI[0] <= 330)
			{

			}
        }

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

			if (NPC.Distance(player.Center) <= 500f)
			{

			}
		}
    }
}