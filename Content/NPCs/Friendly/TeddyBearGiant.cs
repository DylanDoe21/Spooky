using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Friendly
{
	public class TeddyBearGiant : ModNPC
	{
		public override void SetStaticDefaults()
		{
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 60;
            NPC.damage = 0;
			NPC.defense = 10;
			NPC.width = 68;
			NPC.height = 78;
            NPC.npcSlots = 0.5f;
			NPC.friendly = true;
			NPC.immortal = true;
            NPC.noGravity = false;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			NPC.HitSound = SoundID.DD2_GoblinScream with { Pitch = 1.25f, Volume = 0.4f };
            NPC.DeathSound = SoundID.NPCDeath1;
			NPC.aiStyle = -1;
		}
        
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CheckActive()
		{
			return false;
		}

        public override void AI()
        {
            NPC.direction = -1;
        }
	}
}