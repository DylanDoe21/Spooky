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

            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 5;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 2;
            NPC.height = 2;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.alpha = 255;
            NPC.aiStyle = -1;
        }

        public override bool CheckActive()
        {
            return NPC.ai[0] < 2;
        }

        public override void AI()
        {
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            switch ((int)NPC.ai[0])
            {
                //fly upward until it hits a valid ceiling 
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

                //spawn the sphider once a ceiling is found
                case 1: 
                {
                    NPC.velocity *= 0;

                    //this "limit" makes it so if the ceiling this npc found is too low to the ground, it will just vanish and not spawn the spider
                    if (NPC.ai[1] < 90)
                    {
                        NPC.active = false;
                    }
                    else if((NPC.Distance(player.Center) <= 300f || player.GetModPlayer<SpookyPlayer>().WhipSpiderAggression) && NPC.ai[1] >= 90)
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
