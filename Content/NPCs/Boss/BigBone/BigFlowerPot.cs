using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigFlowerPot : ModNPC  
    {
        Vector2 SaveNPCPosition;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 92;
            NPC.height = 94;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            if (NPC.ai[1] == 1)
            {
                NPC.ai[0]++;

                if (NPC.ai[0] == 1)
                {
                    SaveNPCPosition = NPC.Center;
                    
                    //shoot dirt particles up
                    for (int numDusts = 0; numDusts < 15; numDusts++)
                    {                                                                                  
                        int dirtDust = Dust.NewDust(new Vector2(NPC.Center.X + Main.rand.Next(-60, 10), NPC.Center.Y - 90), 
                        NPC.width / 2, NPC.height / 2, DustID.Dirt, 0f, -2f, 0, default, 1.5f);

                        Main.dust[dirtDust].noGravity = false;
                        Main.dust[dirtDust].velocity.Y *= Main.rand.Next(10, 20);
                        
                        if (Main.dust[dirtDust].position != NPC.Center)
                        {
                            Main.dust[dirtDust].velocity = NPC.DirectionTo(Main.dust[dirtDust].position) * 2f;
                        }
                    }

                    NPC.netUpdate = true;
                }

                if (NPC.ai[0] >= 60 && NPC.ai[0] < 180)
                {
                    NPC.Center = new Vector2(SaveNPCPosition.X, SaveNPCPosition.Y);
                    NPC.Center += Main.rand.NextVector2Square(-5, 5);

                    //shoot dirt particles up
                    for (int numDusts = 0; numDusts < 2; numDusts++)
                    {                                                                                  
                        int dirtDust = Dust.NewDust(new Vector2(NPC.Center.X + Main.rand.Next(-60, 10), NPC.Center.Y - 90), 
                        NPC.width / 2, NPC.height / 2, DustID.Dirt, 0f, -2f, 0, default, 1.5f);
                        Main.dust[dirtDust].noGravity = false;
                        Main.dust[dirtDust].velocity.Y *= Main.rand.Next(10, 20);
                        
                        if (Main.dust[dirtDust].position != NPC.Center)
                        {
                            Main.dust[dirtDust].velocity = NPC.DirectionTo(Main.dust[dirtDust].position) * 2f;
                        }
                    }

                    NPC.netUpdate = true;
                }

                if (NPC.ai[0] == 180)
                {
                    NPC.Center = SaveNPCPosition;

                    NPC.netUpdate = true;
                }

                if (NPC.ai[0] == 240)
                {
                    //spawn message
                    string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.BigBoneSpawn");

                    if (Main.netMode != NetmodeID.Server)
                    {
                        Main.NewText(text, 171, 64, 255);
                    }
                    else
                    {
                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                    }
                    
                    NPC.ai[2] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BigBone>(), ai3: NPC.whoAmI);
                    
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);

                    NPC.netUpdate = true;

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NPC.netUpdate = true;
                }
            }
        }
    }
}