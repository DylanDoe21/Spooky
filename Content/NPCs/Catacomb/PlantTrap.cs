using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb
{
    public class PlantTrap : ModNPC  
    {
        public bool Biting = false;

        public static readonly SoundStyle ChompSound = new("Spooky/Content/Sounds/Chomp", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toothy");
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = Main.hardMode ? 175 : 60;
            NPC.damage = Main.hardMode ? 70 : 45;
            NPC.defense = Main.hardMode ? 22 : 15;
            NPC.width = 43;
            NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("These things will trap anything in their mouth that moves above them, living or not. Probably not that useful considering they live in an abandoned catacomb."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (player.InModBiome(ModContent.GetInstance<Biomes.CatacombBiome>()))
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

                if (NPC.frameCounter > 2)
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

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            Biting = true;

            if (NPC.localAI[0] != 1)
            {
                SoundEngine.PlaySound(ChompSound, NPC.Center);
                NPC.localAI[0] = 1;
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 50));
        }

        public override bool CheckDead() 
		{
            if (Main.netMode != NetmodeID.Server) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/PlantTrapGore" + numGores).Type);
                }
            }

            return true;
		}
    }
}