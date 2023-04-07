using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.Cemetery
{
	public class ChalupoBaby : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 65;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 24;
			NPC.height = 28;
            NPC.npcSlots = 1;
            NPC.noGravity = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = 114;
			AIType = NPCID.BlackDragonfly;
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (!spawnInfo.Invasion && Main.invasionType == 0 && !Main.pumpkinMoon && !Main.snowMoon && !Main.eclipse &&
            !(player.ZoneTowerSolar || player.ZoneTowerVortex || player.ZoneTowerNebula || player.ZoneTowerStardust))
            {
                if (player.InModBiome(ModContent.GetInstance<Biomes.CemeteryBiome>()))
                {
                    return 15f;
                }
            }

            return 0f;
        }
        
        public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 1;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0;
            }
		}

        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);
            
            NPC.spriteDirection = NPC.direction;
            NPC.rotation = NPC.velocity.X * 0.05f;

            if (player.Distance(NPC.Center) <= 200f)
            {
                SoundEngine.PlaySound(SoundID.Zombie7, NPC.Center);

                NPC.Transform(ModContent.NPCType<ChalupoBig>());

                for (int numDust = 0; numDust < 12; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[DustGore].noGravity = true;
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 12; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.RedTorch, 0f, -2f, 0, default(Color), 1.5f);
                    Main.dust[DustGore].noGravity = true;
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                }
            }
        }
	}
}