using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Catacomb.Layer1
{
    public class BoneStacker1 : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 120;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.width = 44;
			NPC.height = 16;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.behindTiles = true;
            NPC.HitSound = SoundID.DD2_SkeletonHurt;
            NPC.aiStyle = 3;
			AIType = NPCID.ZombieMushroom; 
        }

        public override bool PreAI()
        {
            //if the base dies, then spawn the npc itself
			if (!Main.npc[(int)NPC.ai[1]].active)
            {
                //chance to spawn the enemy itself, otherwise spawn gores and die
                if (Main.rand.NextBool())
                {

                }
                else
                {
                    /*
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ValleyEelBodyGore1").Type);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ValleyEelBodyGore2").Type);
                    }
                    */
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
                NPC.position.Y = NPC.position.Y - 0.1f + posY;
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

    public class BoneStacker2 : BoneStacker1
    {
    }

    public class BoneStacker3 : BoneStacker1
    {
    }
}
