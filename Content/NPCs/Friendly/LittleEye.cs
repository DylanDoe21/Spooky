using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.BossSummon;
using Spooky.Content.NPCs.Boss.Orroboro;
using Spooky.Content.NPCs.EggEvent;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleEye : ModNPC
	{
		public static int ChosenQuestForToday = 0;

        int Quest1Timer = 0;
		int Quest2Timer = 0;
		int Quest3Timer = 0;
		int Quest4Timer = 0;
		int Quest5Timer = 0;
		int RandomQuestTimer = 0;

        private static int ShimmerHeadIndex;
        private static Profiles.StackedNPCProfile NPCProfile;

        public override void SetStaticDefaults()
		{
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            Main.npcFrameCount[NPC.type] = 5;

            NPCProfile = new Profiles.StackedNPCProfile(new Profiles.DefaultNPCProfile(Texture, NPCHeadLoader.GetHeadSlot(HeadTexture)), new Profiles.DefaultNPCProfile(Texture + "_Shimmer", ShimmerHeadIndex));
        }

        public override void Load()
        {
            ShimmerHeadIndex = Mod.AddNPCHeadTexture(Type, Texture + "_Shimmer_Head");
        }

        public override ITownNPCProfile TownNPCProfile()
        {
            return NPCProfile;
        }

        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
            TownNPCStayingHomeless = true;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
            NPC.aiStyle = 7;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<Biomes.SpookyHellBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.LittleEye"),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<Biomes.SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Button1");
			button2 = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Button2");
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shopName)
		{
			//quest button
			if (firstButton) 
            {
				LittleEyeQuestUI.LittleEye = NPC.whoAmI;
                LittleEyeQuestUI.UIOpen = true;
			}
			//cauldron button
			else
			{
				Main.npcChatText = Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Upgrades");
			}
		}

		public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default1"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default2"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default3"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default4"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default5"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default6"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default7"),
				Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.Default8"),
			};

			if (Main.player[Main.myPlayer].HasItem(ModContent.ItemType<Concoction>()) && !EggEventWorld.EggEventActive && !Flags.downedEggEvent)
            {
				return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.EggEventAdvice");
			}

			if (NPC.AnyNPCs(ModContent.NPCType<OrroHeadP1>()) || NPC.AnyNPCs(ModContent.NPCType<OrroHead>()) || NPC.AnyNPCs(ModContent.NPCType<BoroHead>()))
            {
				return Language.GetTextValue("Mods.Spooky.Dialogue.LittleEye.OrroBoro");
			}

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			NPC.TargetClosest(true);
            Player player = Main.player[NPC.target];

            NPC.spriteDirection = NPC.direction;

            NPC.velocity.X *= 0;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}