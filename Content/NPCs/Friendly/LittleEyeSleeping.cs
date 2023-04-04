using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Personalities;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.Friendly
{
	public class LittleEyeSleeping : ModNPC
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sleeping Eye");
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
			NPCID.Sets.NPCBestiaryDrawModifiers value = new(0) { Hide = true };
			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 40;
			NPC.friendly = true;
			NPC.townNPC = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.rarity = 1;
            NPC.aiStyle = -1;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override string GetChat()
		{
			NPC.Transform(ModContent.NPCType<LittleEye>());
			return "Whoa there, I was trying to take a nap! You must be a pretty curious adventurer to have found your way down here.";
		}

		public override void AI()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient) 
            {
				NPC.homeless = false;
				NPC.homeTileX = -1;
				NPC.homeTileY = -1;
				NPC.netUpdate = true;
			}

            foreach (var player in Main.player)
            {
                if (!player.active) continue;
                if (player.talkNPC == NPC.whoAmI)
                {
                    NPC.Transform(ModContent.NPCType<LittleEye>());
                    return;
                }
            }
        }
    }
}