using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Microsoft.Xna.Framework;

namespace Spooky.Content.NPCs.Friendly
{
	public class LittleBoneSleeping : ModNPC
	{
		public override void SetStaticDefaults()
		{
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

			NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
		}

		public override void SetDefaults()
		{
			NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 22;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
			TownNPCStayingHomeless = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
		}

		public override bool CanBeHitByNPC(NPC attacker)
		{
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return false;
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override bool CanChat() 
        {
			return true;
		}

		public override string GetChat()
		{
            return Language.GetTextValue("Mods.Spooky.Dialogue.LittleBone.Awaken");
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
                    NPC.Transform(ModContent.NPCType<LittleBone>());
                    return;
                }
            }
        }
    }
}