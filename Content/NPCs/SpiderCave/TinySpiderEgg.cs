using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace Spooky.Content.NPCs.SpiderCave
{
	public class TinySpiderEgg : ModNPC
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
			NPC.width = 26;
			NPC.height = 20;
            NPC.npcSlots = 1f;
            NPC.value = Item.buyPrice(0, 0, 0, 0);
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = 0;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (Vector2.Distance(player.Center, NPC.Center) <= 250f)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);

                int[] Spiders = new int[] { ModContent.NPCType<TinySpider1>(), ModContent.NPCType<TinySpider2>(), ModContent.NPCType<TinySpider3>(), ModContent.NPCType<TinySpider4>(), ModContent.NPCType<TinySpider5>() };

                int NewEnemy = NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, Main.rand.Next(Spiders));

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NetMessage.SendData(MessageID.SyncNPC, number: NewEnemy);
                }

                for (int numDusts = 0; numDusts < 20; numDusts++)
                {
                    int dustGore = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Web, 0f, -2f, 0, default, 1f);
                    Main.dust[dustGore].velocity.X *= Main.rand.NextFloat(-5f, 5f);
                    Main.dust[dustGore].velocity.Y *= Main.rand.NextFloat(-3f, 3f);
                    Main.dust[dustGore].noGravity = true;
                }

                NPC.active = false;
            }
        }
	}
}