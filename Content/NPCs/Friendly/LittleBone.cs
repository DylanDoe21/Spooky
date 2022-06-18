using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Biomes;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleBone : ModNPC
	{
		public static bool HasGivenEye;
		public static int NumGoodsGiven = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Skull Plant");
			NPCID.Sets.TownCritter[NPC.type] = true;
            Main.npcFrameCount[NPC.type] = 8;

			NPC.Happiness
			.SetBiomeAffection<Content.Biomes.SpookyBiome>(AffectionLevel.Love) 
			.SetBiomeAffection<ForestBiome>(AffectionLevel.Like)
			.SetBiomeAffection<SnowBiome>(AffectionLevel.Dislike)
			.SetNPCAffection(NPCID.Dryad, AffectionLevel.Love)
			.SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Like)
			.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) 
			.SetNPCAffection(NPCID.TravellingMerchant, AffectionLevel.Hate);
		}

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), //Plain black background
				new FlavorTextBestiaryInfoElement("A cute and friendly little skull creature who hangs around in his flower pot. He likes to give advice to passerbys")
			});
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
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

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 8)
            {
                NPC.frame.Y = 0;
            }
        }

		public override List<string> SetNPCNameList()
        {
            string[] names = { "Little Bone" };

            return new List<string>(names);
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Advice";
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton) 
            {
				//spooky biome
				if (!NPC.downedBoss1)
				{
					Main.npcChatText = "Ahhh, it seems you are still beginning your adventure, but thats ok! As a matter of fact, I have heard that underground beneath this forest, you can find special loot! I am not too sure who left it there, though.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//rot gourd
				else if (NPC.downedBoss1 && !Flags.downedRotGourd)
				{
					Main.npcChatText = "You are progressing pretty fast! I remember hearing of a giant, hostile pumpkin who used to guard this biome. Maybe you can try breaking the pumpkins in this spooky forest?";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				//spooky hell
				else if (NPC.downedBoss2)
				{
					Main.npcChatText = "Somewhere down in the underworld, you can find a really creepy biome filled with eyes! I do not like it there because it feels like I am being watched, but you look strong enough to explore it.";
					SoundEngine.PlaySound(SoundID.Item56, NPC.Center);
				}
				else
				{
					Main.npcChatText = "Sorry, I do not have any advice to offer right now, come back later!";
					SoundEngine.PlaySound(SoundID.Item16, NPC.Center);
				}
            }
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{	
				"I wish I could go outside to play in the leaves, but I also can't get out of this pot! Oh well...",
				"I am not sure how these weird houses got here, but maybe you can fix them up? I would do it myself, but...",
                "Sometimes I like to talk to the other skull plants if they pop up nearby this house, but they don't seem to respond to me.",
				"Why am I always dancing you ask? I like to vibe with the season. You should try it sometime!",
			};

            return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			NPC.homeTileX = (int)NPC.Center.X;
			NPC.homeTileY = (int)NPC.Center.Y;
		}

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}