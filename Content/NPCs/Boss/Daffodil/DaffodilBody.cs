using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
	public class DaffodilBody : ModNPC
	{
		public override void SetDefaults()
		{
            NPC.lifeMax = 18000;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 518;
            NPC.height = 272;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
		}

		public override bool NeedSaving()
        {
            return true;
        }

        public override void AI()
        {
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

                int DaffodilEye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 24, ModContent.NPCType<DaffodilEye>());
                
                if (Main.netMode != NetmodeID.SinglePlayer)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: DaffodilEye);
                }

                NPC.ai[0] = 0;

                NPC.netUpdate = true;
            }
        }
    }
}