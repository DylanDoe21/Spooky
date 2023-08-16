using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
	public class DaffodilBody : ModNPC
	{
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 18000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 530;
            NPC.height = 270;
            NPC.knockBackResist = 0f;
            NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.behindTiles = true;
            TownNPCStayingHomeless = true;
            NPC.aiStyle = -1;
		}

        public override bool NeedSaving()
        {
            return true;
        }

        public override void AI()
        {
            //sleepy particles
            if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()))
            {
                if (!Main.gamePaused)
                {
                    if (Main.rand.NextBool(75))
                    {
                        Dust.NewDust(new Vector2(NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-25, 25)), 5, 5, ModContent.DustType<DaffodilSleepyDust>());
                    }
                }
            }

            //spawn daffodil eye if awakened
            if (NPC.ai[0] == 1)
            {
                //spawn message
                string text = Language.GetTextValue("Mods.Spooky.EventsAndBosses.DaffodilSpawn");

                if (Main.netMode != NetmodeID.Server)
                {
                    Main.NewText(text, 171, 64, 255);
                }
                else
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                }

                NPC.ai[1] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 30, ModContent.NPCType<DaffodilEye>(), ai1: NPC.whoAmI);
                Main.npc[(int)NPC.ai[1]].ai[0] = -1;
                
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[1]);
                }

                NPC.ai[0] = 0;

                NPC.netUpdate = true;
            }
        }
    }
}