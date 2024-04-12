using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Spooky.Content.NPCs.Catacomb.Layer2
{
    public class OrchidStem : ModNPC  
    {
        public override string Texture => "Spooky/Content/NPCs/Catacomb/Layer2/OrchidStem1";

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
            NPC.width = 20;
			NPC.height = 12;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.behindTiles = true;
            NPC.alpha = 255;
        }

        public override void AI()
        {
            NPC.velocity *= 0;

            if (NPC.ai[0] == 0)
            {
                int[] OrchidTypes = new int[] { ModContent.NPCType<OrchidPinkBig>(), ModContent.NPCType<OrchidPinkSmall>(), ModContent.NPCType<OrchidPurpleBig>(), ModContent.NPCType<OrchidPurpleSmall>() };

                //spawn the orchid immediately
                int Orchid = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, Main.rand.Next(OrchidTypes), ai0: NPC.whoAmI);
                        
                NetMessage.SendData(MessageID.SyncNPC, number: Orchid);

                //increase the y position so the stem looks like its coming from the ground
                NPC.position.Y += 40;

                NPC.ai[0] = 1;

                NPC.netUpdate = true;
            }
        }
    }
}