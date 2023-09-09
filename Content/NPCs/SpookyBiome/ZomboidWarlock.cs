using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Food;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class ZomboidWarlock : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 9;
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
            NPC.lifeMax = 100;
            NPC.damage = 22;
            NPC.defense = 5;
            NPC.width = 46;
			NPC.height = 56;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 75);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            SpawnModBiomes = new int[2] { ModContent.GetInstance<Biomes.SpookyBiome>().Type, ModContent.GetInstance<Biomes.SpookyBiomeUg>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.ZomboidWarlock"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiomeUg>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            //running animation
            NPC.frameCounter += 1;

            //use regular walking anim when in walking state
            if (NPC.localAI[0] <= 420)
            {
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping frame when falling/jumping
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 8 * frameHeight;
                }
            }
            //use casting animation during casting ai
            if (NPC.localAI[0] > 420)
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }

                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 7 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            int Damage = Main.masterMode ? 30 / 3 : Main.expertMode ? 25 / 2 : 12;

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 420)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }

            if (NPC.localAI[0] > 420)
            {
                NPC.aiStyle = 0;

                if (NPC.localAI[0] == 480 || NPC.localAI[0] == 500 || NPC.localAI[0] == 520)
                {
                    SoundEngine.PlaySound(SoundID.Item8, NPC.Center);

                    Vector2 ShootSpeed = player.Center - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed.X *= 4.5f;
                    ShootSpeed.Y *= 4.5f;
                    
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y - 20, ShootSpeed.X, 
                    ShootSpeed.Y, ModContent.ProjectileType<WarlockSkull>(), Damage, 1, NPC.target, 0, 0);
                }
            }

            if (NPC.localAI[0] >= 560)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WarlockHood>(), 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SkullWispStaff>(), 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<WarlockRobe>(), 6));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FrankenMarshmallow>(), 50));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 5; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ZomboidWarlockGore" + numGores).Type);
                    }
                }
            }
        }
    }
}