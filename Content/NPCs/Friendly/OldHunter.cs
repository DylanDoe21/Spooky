using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Cemetery.Contraband;
using Spooky.Content.Items.Pets;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
    public class OldHunter : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.defense = 5;
            NPC.width = 38;
			NPC.height = 56;
            NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			TownNPCStayingHomeless = true;
			NPC.knockBackResist = 0.75f;
            NPC.HitSound = SoundID.NPCHit2;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 7;
        }

        public override void FindFrame(int frameHeight)
        {   
            //walking animation
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 7)
            {
                NPC.frame.Y = 1 * frameHeight;
            }

            //still frame
            if (NPC.velocity == Vector2.Zero)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CanChat() 
        {
			return true;
		}

		public override string GetChat()
		{
			OldHunterDialogueChoiceUI.OldHunter = NPC.whoAmI;
			OldHunterDialogueChoiceUI.UIOpen = true;
            return string.Empty;
		}

		public override bool CheckActive()
		{
			return false;
		}

		public override bool NeedSaving()
		{
			return true;
		}
        
        public override void AI()
		{
			NPC.spriteDirection = NPC.direction;

            if ((NPC.velocity.X < 0f && NPC.direction == -1) || (NPC.velocity.X > 0f && NPC.direction == 1))
            {
                if (NPCGlobalHelper.IsCollidingWithFloor(NPC) && Collision.SolidTilesVersatile((int)(NPC.Center.X / 16f), (int)(NPC.Center.X + NPC.spriteDirection * 35) / 16, (int)NPC.Top.Y / 16, (int)NPC.Bottom.Y / 16 - 3))
                {
                    NPC.velocity.X = -NPC.velocity.X;
                    NPC.direction = -NPC.direction;
                    NPC.netUpdate = true;
                }
            }
        }
    }
}