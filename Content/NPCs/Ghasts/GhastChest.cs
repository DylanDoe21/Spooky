using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Spooky.Content.NPCs.Ghasts
{
    public class GhastChest : ModNPC  
    {
        public int WavePoints = 0;
        public bool ActivateGhostEvent = false;
        public bool GhostEventActive = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghostly Chest");
            Main.npcFrameCount[NPC.type] = 3;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 200;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 46;
            NPC.height = 38;
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

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Spooky/Content/NPCs/Ghasts/GhastChestGlow").Value;

            var effects = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            float fade = (float)Math.Cos((double)(Main.GlobalTimeWrappedHourly % 2.5f / 2.5f * 6.28318548f)) / 2f + 0.5f;

            Color color = Color.Lerp(Color.Lerp(new Color(0, 255, 235), new Color(0, 174, 255), fade), new Color(0, 84, 255), fade);

            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY + 4), null, color, NPC.rotation, NPC.frame.Size() / 2f, NPC.scale, effects, 0);
        }

        public override string GetChat()
		{
            return "It's a strange chest. Maybe it will do something in the future?";
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                ActivateGhostEvent = true;
            }
        }

        public override void AI()
        {
            if (ActivateGhostEvent)
            {
                GhostEventActive = true;

                //spawn whatever npcs in waves

                //wave one: 4 bobbert
                //wave two: 2 bobbert, 2 stitch
                //wave three: 2 stitch, 2 sheldon
                //wave 4: 2 sheldon, one chester
            }
            else
            {
                GhostEventActive = false;
            }

            /*
            Main.NewText("Big Bone has awoken!", 171, 64, 255);
            int BigBone = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2),
            (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<BigBone>(), NPC.whoAmI);
            Main.npc[BigBone].ai[3] = NPC.whoAmI;
            Main.npc[BigBone].netUpdate = true;
            BigBoneSpawned = true;

            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, BigBone);
            }
            */

            NPC.netUpdate = true;
        }
    }
}