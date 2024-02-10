using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class TinySpiderEgg : ModNPC
	{
		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[NPC.type] = 3;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 20;
            NPC.damage = 10;
			NPC.defense = 0;
			NPC.width = 26;
			NPC.height = 20;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 50);
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit29;
			NPC.DeathSound = SoundID.NPCDeath32;
            NPC.aiStyle = 0;
        }

        public override void HitEffect(NPC.HitInfo hit) 
        {
            if (NPC.life <= 0) 
            {
                int[] Spiders = new int[] { ModContent.NPCType<TinySpider1>(), ModContent.NPCType<TinySpider2>(), ModContent.NPCType<TinySpider3>(), ModContent.NPCType<TinySpider4>(), ModContent.NPCType<TinySpider5>() };

                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Web, 0f, -2f, 0, default, 1f);
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }
            }
        }
	}
}