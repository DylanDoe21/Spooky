using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.NPCs.SpookyHell
{
    public class MoclingSwarm : ModNPC
    {
        public override string Texture => "Spooky/Content/Projectiles/Blank";

        int GroupAmount = Main.rand.Next(3, 7);

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
            NPC.width = 10;
            NPC.height = 10;
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
            NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            if (NPC.ai[0] == 0)
            {
                for (int numFlies = 0; numFlies < GroupAmount; numFlies++)
                {
                    int NewMocling = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Mocling>(), ai1: NPC.whoAmI);

                    if (Main.netMode != NetmodeID.SinglePlayer)
                    {
                        NetMessage.SendData(MessageID.SyncNPC, number: NewMocling);
                    }
                }

                NPC.ai[0]++;
            }

            for (int i = 0; i < Main.maxPlayers; i++)
			{
                if (Main.player[i].Distance(NPC.Center) <= 250f)
                {
                    NPC.active = false;
                }
            }
        }
    }
}
