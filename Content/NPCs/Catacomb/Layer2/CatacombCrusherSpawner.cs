using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class CatacombCrusherSpawner : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 10;
            NPC.height = 10;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            switch ((int)NPC.ai[0])
            {
                //fly upward until it hits a valid ceiling 
                case 0: 
                {
                    NPC.velocity.Y = -10;

                    NPC.ai[1]++;

                    if (NPC.ai[1] >= 10 && Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                    {
                        NPC.ai[0]++;
                    }

                    break;
                }

                //spawn the crusher once the ceiling is found
                case 1: 
                {
                    if (NPC.ai[1] < 22)
                    {
                        NPC.active = false;
                    }

                    if (NPC.ai[1] >= 22)
                    {
                        if (Main.rand.NextBool(10))
                        {
                            int Crusher = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 78, ModContent.NPCType<CatacombCrusher4>());

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {   
                                NetMessage.SendData(MessageID.SyncNPC, number: Crusher);
                            }
                        }
                        else
                        {
                            int[] CrusherTypes = new int[] { ModContent.NPCType<CatacombCrusher1>(), ModContent.NPCType<CatacombCrusher2>(), ModContent.NPCType<CatacombCrusher3>() };

                            int Crusher = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 78, Main.rand.Next(CrusherTypes));

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {   
                                NetMessage.SendData(MessageID.SyncNPC, number: Crusher);
                            }
                        }

                        NPC.netUpdate = true;

                        NPC.active = false;
                    }

                    break;
                }
            }
        }
    }
}
