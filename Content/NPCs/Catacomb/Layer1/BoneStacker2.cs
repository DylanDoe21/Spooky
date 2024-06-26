using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Content.Items.Food;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class BoneStacker2 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 50;
            NPC.damage = 15;
            NPC.defense = 15;
            NPC.width = 44;
			NPC.height = 16;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
        }

        public override bool PreAI()
        {
            //if the base dies, then spawn the npc itself
			if (!Main.npc[(int)NPC.ai[1]].active)
            {
                //chance to spawn the enemy itself, otherwise spawn gores and die
                if (Main.rand.NextBool())
                {
                    int newStacker = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + (NPC.width / 2) + Main.rand.Next(-20, 20), 
                    (int)NPC.Center.Y + (NPC.height / 2), ModContent.NPCType<BoneStackerMoving2>(), NPC.whoAmI);
                    
                    if (Main.netMode != NetmodeID.MultiplayerClient) 
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: newStacker);
                    }
                }
                else
                {
                    for (int numGores = 1; numGores <= 2; numGores++)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneStackerFlowerGore" + numGores).Type);
                        }
                    }
                }

                NPC.active = false;
            }
			
			if (NPC.ai[1] < (double)Main.npc.Length)
            {
                Vector2 npcCenter = new(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float dirX = Main.npc[(int)NPC.ai[1]].position.X + (float)(Main.npc[(int)NPC.ai[1]].width / 2) - npcCenter.X;
                float dirY = Main.npc[(int)NPC.ai[1]].position.Y + (float)(Main.npc[(int)NPC.ai[1]].height / 2) - npcCenter.Y;
                float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                float dist = (length - (float)NPC.height) / length;
                float posX = dirX * dist;
                float posY = dirY * dist;

                NPC.spriteDirection = Main.npc[(int)NPC.ai[1]].spriteDirection;
                NPC.velocity = Vector2.Zero;
                NPC.position.X = NPC.position.X + posX;
                NPC.position.Y = NPC.position.Y - 0.3f + posY;
            }

			return false;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }
    }

    public class BoneStackerMoving2 : BoneStackerMoving1
    {
        public override string Texture => "Spooky/Content/NPCs/Catacomb/Layer1/BoneStacker2";

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.BoneStacker2"),
                new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.CatacombBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void HitEffect(NPC.HitInfo hit) 
        {
			if (NPC.life <= 0) 
            {
                for (int numGores = 1; numGores <= 2; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/BoneStackerFlowerGore" + numGores).Type);
                    }
                }
            }
        }
    }
}
