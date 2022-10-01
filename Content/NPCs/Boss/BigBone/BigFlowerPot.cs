using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;

using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigFlowerPot : ModNPC  
    {
        public static bool ShouldSpawnBigBone = false;

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
            NPC.height = 92;
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

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{	
				"It's a giant flower pot. Perhaps a certain fertalizer may re-awaken the plant inside of it?",
				"A giangantic flower pot, seems like whatever was inside isn't very alive anymore.",
                "Why are you talking to a giant flower pot? Weirdo.",
			};

            return Main.rand.Next(Dialogue);
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override void AI()
        {
            if (NPC.ai[1] == 1)
            {
                if (Main.netMode != 1)
                {
                    NPC.ai[3] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BigBone>(), ai3: NPC.whoAmI);
                }

                //net update so it doesnt vanish on multiplayer
                if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[3]);
                }

                NPC.netUpdate = true;
                NPC.ai[1] = 0;
            }
        }
    }
}