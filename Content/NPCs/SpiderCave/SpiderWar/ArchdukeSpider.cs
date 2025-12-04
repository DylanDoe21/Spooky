using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave.SpiderWar
{
	public class ArchdukeSpider : ModNPC
	{
		public override void SetStaticDefaults()
		{
            Main.npcFrameCount[NPC.type] = 3;
		}

        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.knockBackResist = 0.2f;
			NPC.noGravity = false;
			NPC.dontCountMe = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 7;
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 3)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override void AI()
		{
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;
		}

		public override void HitEffect(NPC.HitInfo hit)
        {
			if (NPC.life <= 0)
            {	
				for (int numGores = 1; numGores <= 4; numGores++)
                {
                    if (Main.netMode != NetmodeID.Server) 
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ArchdukeSpiderGore" + numGores).Type);
                    }
                }

				SpiderWarWorld.SpiderWarActive = true;

				if (Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.WorldData);
				}
			}
		}
	}
}