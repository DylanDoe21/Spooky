using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossBags;
using Spooky.Content.Items.Costume;
using Spooky.Content.Items.Pets;
using Spooky.Content.Items.SpookyBiome;
using Spooky.Content.NPCs.Boss.RotGourd.Projectiles;
using Spooky.Content.Tiles.Relic;
using Spooky.Content.Tiles.Trophy;

namespace Spooky.Content.NPCs.Boss.Daffodil
{
	//[AutoloadBossHead]
	public class Daffodil : ModNPC
	{
		public override void SetDefaults()
		{
            NPC.lifeMax = 2200;
            NPC.damage = 18;
            NPC.defense = 5;
            NPC.width = 518;
            NPC.height = 314;
            NPC.knockBackResist = 0f;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath1;
            NPC.aiStyle = -1;
		}

		public override bool NeedSaving()
        {
            return true;
        }

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);
		}
    }
}