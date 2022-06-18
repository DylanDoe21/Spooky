using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class SpookyDance : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spooky Dance Ghost");
            Main.npcFrameCount[NPC.type] = 4;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 45;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 40;
			NPC.height = 56;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
            AnimationType = NPCID.Ghost;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("Spooky month lasts all year!")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn on the surface during the day, or underground
                if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyBiome>()))
                {
                    return 1f;
                }
            }

            return 0f;
        }

        public override void AI()
        {
            if (NPC.localAI[0] == 0)
            {
                NPC.ai[0] = NPC.position.Y;
                NPC.localAI[0]++;
            }

            NPC.ai[1]++;
            NPC.position.Y = NPC.ai[0] + (float)Math.Sin(NPC.ai[1] / 30) * 30;
        }

        public override void HitEffect(int hitDirection, double damage)
		{
			if (NPC.life <= 0)
			{
            	for (int numDusts = 0; numDusts < 20; numDusts++)
				{
					int GhostDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 65, 0f, 0f, 100, default(Color), 2f);
					Main.dust[GhostDust].velocity *= 3f;
                    Main.dust[GhostDust].noGravity = true;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[GhostDust].scale = 0.5f;
						Main.dust[GhostDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
			}
		}
    }
}