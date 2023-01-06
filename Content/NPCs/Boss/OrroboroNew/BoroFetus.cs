using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using System.IO;
using System.Collections.Generic;

namespace Spooky.Content.NPCs.Boss.OrroboroNew
{
    public class BoroFetus : ModNPC  
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strange Fetus");
            NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 350;
            NPC.damage = 0;
            NPC.defense = 0;
            NPC.width = 36;
			NPC.height = 36;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
        }
        
        public override void AI()
		{
            Player player = Main.player[NPC.target];
            NPC.TargetClosest(true);

            NPC.localAI[0]++;
            if (NPC.localAI[0] <= 60)
            {
                NPC.velocity.X = -3;
                NPC.velocity.Y = -6;
            }
            else
            {
                NPC.velocity *= 0.85f;
            }
        }

        public override void HitEffect(int hitDirection, double damage) 
        {
            //spawn boro here
			if (NPC.life <= 0) 
            {
            }
        }
    }
}