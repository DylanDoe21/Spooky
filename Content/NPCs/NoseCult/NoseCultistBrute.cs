using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.NoseCult.Projectiles;

namespace Spooky.Content.NPCs.NoseCult
{
    public class NoseCultistBrute : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Position = new Vector2(0f, 50f),
                PortraitPositionXOverride = 0f,
                PortraitPositionYOverride = 10f
            };
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
            NPC.lifeMax = 900;
            NPC.damage = 45;
            NPC.defense = 15;
            NPC.width = 106;
			NPC.height = 112;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 20, 0);
            NPC.HitSound = SoundID.NPCHit48 with { Pitch = -2f };
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 3;
			AIType = NPCID.Crab;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.NoseTempleBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.NoseCultistBrute"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.NoseTempleBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {   
            //running animation
            NPC.frameCounter++;
            if (NPC.velocity.Y == 0)
            {
                if (NPC.frameCounter > 9)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
            //falling animation
            else
            {
                if (NPC.velocity.Y < 0)
                {
                    NPC.frame.Y = 2 * frameHeight;
                }
                else
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
            }
        }
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            if (NPC.localAI[0] == 0)
            {
                NPCGlobalHelper.ShootHostileProjectile(NPC, new Vector2(NPC.Center.X + Main.rand.Next(-75, 75), NPC.Center.Y + Main.rand.Next(-50, -10)), 
                Vector2.Zero, ModContent.ProjectileType<NoseCultistBruteFlail>(), NPC.damage, 4.5f, ai0: NPC.whoAmI);

                NPC.localAI[0] = 1;
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.ByCondition(new DropConditions.PostMocoCondition(), ModContent.ItemType<SnotGlob>(), 1, 2, 6));
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 7; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/NoseCultistBruteGore" + numGores).Type);
                    }
                }
            }
        }
    }
}