using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Items.SpookyBiome.Misc;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class HalloweenWisp : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Halloween Wisp");
			Main.npcFrameCount[NPC.type] = 8;
			Main.npcCatchable[NPC.type] = true;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 18;
			NPC.height = 24;
            NPC.npcSlots = 1f;
            NPC.noGravity = true;
			NPC.catchItem = ModContent.ItemType<HalloweenWispItem>();
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath6;
			NPC.aiStyle = 64;
			AIType = NPCID.Firefly;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("Ethereal wisps that wander aimlessly during the night in the spooky forest.")
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                //spawn on the surface during the night
                if (player.InModBiome(ModContent.GetInstance<Biomes.SpookyBiome>()) && !Main.dayTime)
                {
                    return 18f;
                }
            }

            return 0f;
        }
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1;
            if (NPC.frameCounter > 7)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0;
            }
		}

        public override void AI()
        {
            NPC.spriteDirection = -NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.07f;

            Lighting.AddLight(NPC.Center, 1f, 0.7f, 0f);
        }

        public override bool CheckDead() 
		{
			for (int numDusts = 0; numDusts < 20; numDusts++)
			{
				int DustGore = Dust.NewDust(new Vector2(NPC.Center.X, NPC.Center.Y), NPC.width / 2, NPC.height / 2, DustID.OrangeTorch, 0f, 0f, 100, default, 2f);
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