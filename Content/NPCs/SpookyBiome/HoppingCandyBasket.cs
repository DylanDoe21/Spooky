using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.SpookyBiome
{
	public class HoppingCandyBasket : ModNPC
	{
		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 15;
            NPC.defense = 0;
            NPC.width = 44;
            NPC.height = 30;
			NPC.knockBackResist = 0.5f;
			NPC.value = Item.buyPrice(0, 0, 2, 0);
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 1;
			AIType = NPCID.HoppinJack;
			SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyBiome>().Type, ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.HoppingCandyBasket"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;
		}

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            int[] CandyList1 = new int[]
            { 
                ModContent.ItemType<CandyCorn>(), 
                ModContent.ItemType<CaramelApple>(), 
                ModContent.ItemType<EyeChocolate>()
            };

            int[] CandyList2 = new int[]
            { 
                ModContent.ItemType<FrankenMarshmallow>(), 
                ModContent.ItemType<GoofyPretzel>(), 
                ModContent.ItemType<VampireGummy>()
            };

            npcLoot.Add(ItemDropRule.OneFromOptions(1, CandyList1));
            npcLoot.Add(ItemDropRule.OneFromOptions(1, CandyList2));
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
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/CandyBasketGore" + numGores).Type);
                }

                for (int numDusts = 0; numDusts < 15; numDusts++)
                {
                    int DustGore = Dust.NewDust(NPC.Center, 1, 1, DustID.TintableDust, 0f, 0f, 100, default, 1f);
                    Main.dust[DustGore].color = Color.Orange;
                    Main.dust[DustGore].velocity *= 1.2f;

                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[DustGore].scale = 0.5f;
                        Main.dust[DustGore].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
		}
    }
}