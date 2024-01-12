using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Content.Items.Pets;
using Spooky.Content.NPCs.SpookyBiome.Projectiles;

namespace Spooky.Content.NPCs.SpookyBiome
{
    public class Witch : ModNPC  
    {
        bool CanShootPotion = false;
        bool HasThrownPotion = false;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 11;
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
            NPC.lifeMax = 300;
            NPC.damage = 45;
            NPC.defense = 5;
            NPC.width = 42;
			NPC.height = 54;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 75);
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.Witch"),
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;

            //walking animation
            if (NPC.localAI[0] <= 420 || NPC.localAI[0] >= 500)
            {
                if (NPC.frameCounter > 10)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }

                //jumping/falling frame
                if (NPC.velocity.Y > 0 || NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //throwing animation
            if (NPC.localAI[0] > 420 && NPC.localAI[0] < 500)
            {
                if (NPC.frame.Y < frameHeight * 6)
                {
                    NPC.frame.Y = 6 * frameHeight;
                }

                if (NPC.frameCounter > 5)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y == frameHeight * 8)
                {
                    CanShootPotion = true;
                }
                if (NPC.frame.Y >= frameHeight * 10)
                {
                    HasThrownPotion = false;
                    NPC.frame.Y = 6 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.localAI[0]++;

            if (NPC.localAI[0] <= 420 || NPC.localAI[0] >= 500)
            {
                NPC.aiStyle = 3;
                AIType = NPCID.Crab;
            }

            //throw flasks
            if (NPC.localAI[0] > 420 && NPC.localAI[0] < 500)
            {
                NPC.aiStyle = 0;

                if (CanShootPotion && !HasThrownPotion)
                {
                    SoundEngine.PlaySound(SoundID.Item106, NPC.Center);

                    Vector2 ShootSpeed = new Vector2(player.Center.X, player.Center.Y - 30) - NPC.Center;
                    ShootSpeed.Normalize();
                    ShootSpeed *= 15f;
                    
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, ShootSpeed, ModContent.ProjectileType<WitchPotion>(), NPC.damage / 4, 0, NPC.target);

                    CanShootPotion = false;
                    HasThrownPotion = true;
                }
            }

            if (NPC.localAI[0] >= 550)
            {
                NPC.localAI[0] = 0;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot) 
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CatEyes>(), 30));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 3; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WitchGore" + numGores).Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/WitchClothGore").Type);
                    }
                }
            }
        }
    }
}