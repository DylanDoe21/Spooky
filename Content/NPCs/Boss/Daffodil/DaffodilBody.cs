using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.Chat;
using Microsoft.Xna.Framework;

using Spooky.Core;
using Spooky.Content.Dusts;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
	public class DaffodilBody : ModNPC
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
            NPC.width = 530;
            NPC.height = 270;
            NPC.npcSlots = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.immortal = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            NPC.dontCountMe = true;
            NPC.aiStyle = -1;
		}
        
        public override bool CheckActive()
        {
            return false;
        }

        public override void AI()
        {
            Flags.DaffodilSpawnX = (int)NPC.Center.X;
            Flags.DaffodilSpawnY = (int)NPC.Center.Y + 30;
            Flags.DaffodilParent = NPC.whoAmI;

            //sleepy particles
            if (!NPC.AnyNPCs(ModContent.NPCType<DaffodilEye>()))
            {
                if (!Main.gamePaused)
                {
                    if (Main.rand.NextBool(75))
                    {
                        Dust.NewDust(new Vector2(NPC.Center.X + Main.rand.Next(-50, 50), NPC.Center.Y + Main.rand.Next(-25, 25)), 5, 5, ModContent.DustType<DaffodilSleepyDust>());
                    }
                }
            }
        }
    }
}