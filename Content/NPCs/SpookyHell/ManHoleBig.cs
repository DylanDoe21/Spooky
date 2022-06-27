using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.NPCs.SpookyHell.Projectiles;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class ManHoleBig : ModNPC  
    {
        private bool spawned;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Man Hole");
            Main.npcFrameCount[NPC.type] = 4;

            var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                CustomTexturePath = "Spooky/Content/NPCs/SpookyHell/ManHoleBigBestiary",
                Position = new Vector2(0f, -15f)
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 150;
            NPC.damage = 45;
            NPC.defense = 20;
            NPC.width = 70;
            NPC.height = 40;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Content.Biomes.SpookyHellBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("Uncommonly, some man holes can form a symbiotic relationship. The eye that lives inside it will detect and attack intruders, allowing the mouth to hunt prey much easier.")
			});
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

			if (Main.LocalPlayer.InModBiome(ModContent.GetInstance<Content.Biomes.SpookyHellBiome>()))
			{
                return 20f;
            }
            return 0f;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;

            //idle
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

        public override void AI()
        {
            //spawn eye when hit
            if (NPC.life < NPC.lifeMax)
            {
                if (!spawned)
                {
                    NPC.realLife = NPC.whoAmI;
                    int Eye = NPC.whoAmI;
                    
                    Eye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), 
                    (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<ManHoleEye>(), NPC.whoAmI, 0, Eye);
                    Main.npc[Eye].ai[3] = NPC.whoAmI;
                    Main.npc[Eye].netUpdate = true;

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, Eye);
                    }

                    NPC.netUpdate = true;
                    spawned = true;
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CreepyChunk>(), 3, 1, 2));
        }

        public override bool CheckDead() 
		{
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleBigGore1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleBigGore2").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleBigGore3").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ManHoleBigGore4").Type);

            return true;
		}
    }
}