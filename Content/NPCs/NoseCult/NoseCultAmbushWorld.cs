using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.Chat;
using Terraria.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;

namespace Spooky.Content.NPCs.NoseCult
{
	public class NoseCultAmbushWorld : ModSystem
	{
		public static bool AmbushActive;

		public override void OnWorldLoad()
		{
			AmbushActive = false;
		}

        public override void PostUpdateEverything()
		{
			//end the event and reset everything if you die
			if (AmbushActive && Main.player[Main.myPlayer].dead)
			{
				AmbushActive = false;
			}

			//bool to check if any cultist enemies exist
            bool AnyCultistsExist = NPC.AnyNPCs(ModContent.NPCType<NoseCultistBrute>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistGrunt>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistGunner>()) || 
            NPC.AnyNPCs(ModContent.NPCType<NoseCultistMage>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistWinged>()) || NPC.AnyNPCs(ModContent.NPCType<NoseCultistLeader>());

			if (!Main.player[Main.myPlayer].InModBiome(ModContent.GetInstance<NoseTempleBiome>())) //|| !AnyCultistsExist)
			{
				AmbushActive = false;
			}
		}
	}
}