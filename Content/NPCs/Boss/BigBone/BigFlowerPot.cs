using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Generation;
using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigFlowerPot : ModNPC  
    {
        Vector2 SaveNPCPosition;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Flower Pot");
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 1;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 92;
            NPC.height = 94;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
            NPC.townNPC = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath5;
            NPC.aiStyle = -1;
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = "";
        }

        public override string GetChat()
        {
            List<string> Dialogue = new List<string>
            {
                "It's a giant flower pot. Perhaps a certain fertalizer may re-awaken the plant inside of it?",
                "Maybe if you have a special fertalizer, you can regrow whatever is in this flower pot.",
                "A giangantic flower pot, seems like whatever was inside isn't very alive anymore.",
                "Why are you talking to a giant flower pot? Weirdo.",
            };

            return Main.rand.Next(Dialogue);
        }

        public override void AI()
        {
            NPC.life = 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.homeless = false;
                NPC.homeTileX = -1;
                NPC.homeTileY = -1;
                NPC.netUpdate = true;
            }

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
                }

                if (NPC.ai[0] == 180)
                {
                    NPC.Center = SaveNPCPosition;
                }

                if (NPC.ai[0] >= 240)
                {
                    Main.NewText("Big Bone has awoken!", 171, 64, 255);

                    if (Main.netMode != 1)
                    {
                        NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BigBone>(), ai3: NPC.whoAmI);
                    }

                    //net update so it doesnt vanish on multiplayer
                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[3]);
                    }

                    NPC.ai[0] = 0;
                    NPC.ai[1] = 0;
                    NPC.netUpdate = true;
                }
            }
        }
    }
}