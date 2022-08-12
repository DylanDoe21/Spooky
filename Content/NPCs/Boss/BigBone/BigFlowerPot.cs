using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Items.BossSummon;

namespace Spooky.Content.NPCs.Boss.BigBone
{
    public class BigFlowerPot : ModNPC  
    {
        public bool headSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Giant Flower Pot");
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 50000;
            NPC.damage = 0;
            NPC.defense = 99999;
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
            return "It's a giant flower pot. Perhaps if you have a special fertalizer, something may happen...";
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Use Fertalizer";
		}

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                Player player = Main.LocalPlayer;
                if (!player.HasItem(ModContent.ItemType<Concoction>()))
                {
                    Main.npcChatText = "You do not seem to have the special fertalizer";
                }   
                else
                {
                    Main.CloseNPCChatOrSign();
                    SpawnBigBone();
                }
            }
        }

        public void SpawnBigBone()
        {
            Main.NewText("Big Bone has awoken!", 171, 64, 255);
            int BigBone = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2),
            (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<BigBone>(), NPC.whoAmI);
            Main.npc[BigBone].ai[3] = NPC.whoAmI;
            Main.npc[BigBone].netUpdate = true;
            headSpawned = true;

            if (Main.netMode == 2)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, BigBone);
            }

            NPC.netUpdate = true;
        }

        /*
        public override void AI()
        {
            if (!headSpawned && NPC.active)
            {
                int BigBone = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2),
                (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<BigBone>(), NPC.whoAmI);
                Main.npc[BigBone].ai[3] = NPC.whoAmI;
                Main.npc[BigBone].netUpdate = true;
                headSpawned = true;

                if (Main.netMode == 2)
                {
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, BigBone);
                }
                
                NPC.netUpdate = true;
            }

            if (NPC.AnyNPCs(ModContent.NPCType<BigBone>()))
            {
                NPC.townNPC = false;
            }
        }
        */
    }
}