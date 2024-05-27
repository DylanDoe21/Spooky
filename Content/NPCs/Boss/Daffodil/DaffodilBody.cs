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
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

		public override void SetDefaults()
		{
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 530;
            NPC.height = 270;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.aiStyle = -1;
		}

        public override bool NeedSaving()
        {
            return true;
        }

        public override void AI()
        {
            Spooky.DaffodilSpawnX = (int)NPC.Center.X;
            Spooky.DaffodilSpawnY = (int)NPC.Center.Y + 30;
            Spooky.DaffodilParent = NPC.whoAmI;

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

                if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()))
                {
                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)SpookyMessageType.SpawnDaffodilEye);
                        packet.Send();

                        ChatHelper.BroadcastChatMessage(NetworkText.FromKey(text), new Color(171, 64, 255));
                    }
                    else
                    {
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 30, ModContent.NPCType<DaffodilEye>(), ai0: Main.rand.NextBool(20) && Flags.downedDaffodil ? -4 : -1, ai1: NPC.whoAmI);

                        Main.NewText(text, 171, 64, 255);
                    }
                }

                NPC.ai[0] = 0;

                NPC.netUpdate = true;
            }
        }
    }
}