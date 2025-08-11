using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Minibiomes.Christmas
{
	public class PresentTrapBlue : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 2;
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 0;
			NPC.defense = 0;
			NPC.width = 46;
			NPC.height = 46;
            NPC.npcSlots = 0f;
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
		}
        
        public override void FindFrame(int frameHeight)
		{
            if (NPC.ai[0] == 0)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else
            {
               NPC.frame.Y = 1 * frameHeight; 
            }
		}

        public override void AI()
        {
            foreach (var player in Main.ActivePlayers)
            {
                if (NPC.Distance(player.Center) <= 150f)
                {
                    if (NPC.ai[0] == 0)
                    {
                        if (Main.netMode != NetmodeID.Server) 
                        {
                            Gore.NewGorePerfect(NPC.GetSource_FromAI(), NPC.position, new Vector2(Main.rand.Next(-2, 3), -3), ModContent.Find<ModGore>("Spooky/PresentTrapBlueLidGore").Type);
                        }

                        NPC.ai[0] = 1;
                    }
                }
            }

            if (NPC.ai[0] == 0)
            {
            }
            else
            {
            }
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                if (Main.netMode != NetmodeID.Server) 
                {
                    //Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, ModContent.Find<ModGore>("Spooky/ShroomHopperGore").Type);
                }
            }
        }
	}
}