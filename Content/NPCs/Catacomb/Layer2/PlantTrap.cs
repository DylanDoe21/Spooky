using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class PlantTrap : ModNPC  
    {
        public bool Biting = false;

        public static readonly SoundStyle ChompSound = new("Spooky/Content/Sounds/Chomp", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Biting);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Biting = reader.ReadBoolean();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 175;
            NPC.damage = 50;
            NPC.defense = 0;
            NPC.width = 43;
            NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.PlantTrap"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome2>()))
            {
                return 15f;
            }

            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            if (!Biting)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 4)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }

            if (Biting)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 1)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 7)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            Biting = true;

            if (NPC.ai[0] != 1)
            {
                NPC.frame.Y = 4;
                SoundEngine.PlaySound(ChompSound, NPC.Center);
                NPC.ai[0] = 1;
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            if (Biting)
            {
                player.Center = NPC.Center;

                if (player.statLife <= 0)
                {
                    Biting = false;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            //dont run on multiplayer
			if (Main.netMode == NetmodeID.Server) 
            {
				return;
			}

			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PlantTrapGore" + numGores).Type);
                }
            }
        }
    }
}