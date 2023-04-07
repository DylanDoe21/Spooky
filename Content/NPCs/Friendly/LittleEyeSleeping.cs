using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Friendly
{
	public class LittleEyeSleeping : ModNPC
	{
		public override void SetStaticDefaults()
		{
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 40;
			NPC.friendly = true;
			NPC.townNPC = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            TownNPCStayingHomeless = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override string GetChat()
		{
			NPC.Transform(ModContent.NPCType<LittleEye>());
			return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Awaken");
		}

        public override void AI()
        {
            NPC.homeless = true;
        }
    }
}