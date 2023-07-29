using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class BoneStackerBase : ModNPC  
    {
        private bool stackersSpawned;
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 120;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.width = 44;
			NPC.height = 16;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 0;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type }; 
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BoneStackerBase"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
			{
                return 30f;
            }
            
            return 0f;
        }

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            NPC.rotation = NPC.velocity.Y * (NPC.direction == 1 ? 0.05f : -0.05f);

            //Create the worm itself
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!stackersSpawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int latestNPC = NPC.whoAmI;

                    for (int numSegments = 0; numSegments < Main.rand.Next(3, 7); numSegments++)
                    {
                        int[] Types = new int[] { ModContent.NPCType<BoneStacker1>(), ModContent.NPCType<BoneStacker2>(), ModContent.NPCType<BoneStacker3>() };

                        latestNPC = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2), (int)NPC.Center.Y + (NPC.height / 2), Main.rand.Next(Types), NPC.whoAmI, 0, latestNPC);
                        Main.npc[latestNPC].ai[3] = NPC.whoAmI;
                        NetMessage.SendData(MessageID.SyncNPC, number: latestNPC);
                    }

                    stackersSpawned = true;
                    NPC.netUpdate = true;
                }
            }
        }

        /*
        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 6; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneWorm1FrontGore" + numGores).Type);
                }
            }
        }
        */
    }
}