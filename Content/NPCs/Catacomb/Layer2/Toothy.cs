using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Catacomb.Misc;
using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class Toothy : ModNPC  
    {
        public bool Biting = false;

        public static readonly SoundStyle ChompSound = new("Spooky/Content/Sounds/Chomp", SoundType.Sound);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 5;

            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
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
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 43;
            NPC.height = 46;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.HitSound = SoundID.Grass;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.CatacombBiome2>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
                new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Toothy"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome2>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            if (!Biting)
            {
                NPC.frame.Y = 0 * frameHeight;
            }

            if (Biting)
            {
                NPC.frameCounter += 1;

                if (NPC.frameCounter > 1)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            if (player.Hitbox.Intersects(NPC.Hitbox) && !Biting)
            {
                Biting = true;
                NPC.frame.Y = 4;
                SoundEngine.PlaySound(ChompSound, NPC.Center);
            }

            if (Biting)
            {
                player.Center = NPC.Center;

                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }

                player.lifeRegen -= 20;

                if (player.statLife <= 0 || player.dead)
                {
                    Biting = false;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<PlantChunk>(), 5, 1, 3));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CandyCorn>(), 100));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numDust = 0; numDust < 12; numDust++)
                {                                                                                  
                    int DustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass, 0f, -2f, 0, default, 1.5f);
                    Main.dust[DustGore].position.X += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                    Main.dust[DustGore].position.Y += Main.rand.Next(-50, 51) * 0.05f - 1.5f;
                }

                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ToothyGore" + numGores).Type);
                    }
                }
            }
        }
    }
}