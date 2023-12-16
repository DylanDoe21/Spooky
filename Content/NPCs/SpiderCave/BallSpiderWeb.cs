using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

using Spooky.Core;

namespace Spooky.Content.NPCs.SpiderCave
{
    public class BallSpiderWeb : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.CantTakeLunchMoney[Type] = true;

            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 2;
            NPC.height = 2;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
        }

        public override void AI()
        {
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            switch ((int)NPC.ai[0])
            {
                case 0: 
                {
                    NPC.velocity.Y = -3;

                    NPC.ai[1]++;

                    if (NPC.ai[1] >= 15 && Collision.SolidCollision(NPC.Center, NPC.width, NPC.height))
                    {
                        NPC.ai[0]++;
                    }

                    break;
                }

                case 1: 
                {
                    NPC.velocity *= 0;

                    if (NPC.ai[1] < 40)
                    {
                        NPC.active = false;
                    }

                    if (NPC.Distance(player.Center) <= 300f || player.GetModPlayer<SpookyPlayer>().WhipSpiderAggression)
                    {
                        SoundEngine.PlaySound(SoundID.Zombie74, NPC.Center);

                        NPC.ai[2] = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<BallSpider>(), ai2: NPC.whoAmI);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {   
                            NetMessage.SendData(MessageID.SyncNPC, number: (int)NPC.ai[2]);
                        }

                        NPC.netUpdate = true;

                        NPC.ai[0]++;
                    }

                    break;
                }
            }
        }
    }
}
