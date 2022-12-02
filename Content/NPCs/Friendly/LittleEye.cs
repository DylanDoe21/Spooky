using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

using Spooky.Content.Biomes;
using Spooky.Content.Items.SpookyHell;

namespace Spooky.Content.NPCs.Friendly
{
    [AutoloadHead]
	public class LittleEye : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alchemist Eye");
			NPCID.Sets.ActsLikeTownNPC[Type] = true;
            Main.npcFrameCount[NPC.type] = 5;
		}

		public override void SetDefaults()
		{
            NPC.lifeMax = 150;
			NPC.damage = 0;
			NPC.defense = 25;
            NPC.width = 20;
			NPC.height = 50;
			NPC.townNPC = true;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0f;
			NPC.rarity = 1;
            NPC.aiStyle = -1;
			SpawnModBiomes = new int[1] { ModContent.GetInstance<SpookyHellBiome>().Type };
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
			bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("A strange little eye stalk with a cool wizard hat. A decently experienced alchemist who prefers using his skills to mess with others."),
				new BestiaryPortraitBackgroundProviderPreferenceInfoElement(ModContent.GetInstance<SpookyHellBiome>().ModBiomeBestiaryInfoElement)
			});
		}

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 1;
            if (NPC.frameCounter > 6)
            {
                NPC.frame.Y = NPC.frame.Y + frameHeight;
                NPC.frameCounter = 0.0;
            }
            if (NPC.frame.Y >= frameHeight * 5)
            {
                NPC.frame.Y = 0;
            }
        }

		public override List<string> SetNPCNameList()
        {
            string[] names = { "Little Eye" };

            return new List<string>(names);
        }

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "Quest";
			button2 = Main.LocalPlayer.HasItem(ModContent.ItemType<OrroboroHeart>()) &&
			(Main.LocalPlayer.HasItem(ModContent.ItemType<FleshAxe>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<FleshBow>()) ||
			Main.LocalPlayer.HasItem(ModContent.ItemType<FleshStaff>()) || Main.LocalPlayer.HasItem(ModContent.ItemType<FleshWhip>())) ? "Sentient Upgrade" : "Upgrades?";
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
		{
			if (firstButton) 
            {
				Main.npcChatText = "Quest dialogue thing";
            }
			else
			{
				//flesh axe
				if (Main.LocalPlayer.HasItem(ModContent.ItemType<OrroboroHeart>()) && Main.LocalPlayer.HasItem(ModContent.ItemType<FleshAxe>()))
				{
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OrroboroHeart>());
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<FleshAxe>());

					Main.npcChatText = "Ooga booga, I proclaim this weapon SENTIENT! I didn't actually have to say that but it sounds cool anyways haha. Anyways have fun smashing stuff with your new axe!";
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<SentientFleshAxe>(), 1);
					SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
				}
				//flesh bow
				else if (Main.LocalPlayer.HasItem(ModContent.ItemType<OrroboroHeart>()) && Main.LocalPlayer.HasItem(ModContent.ItemType<FleshBow>()))
				{
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OrroboroHeart>());
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<FleshBow>());

					Main.npcChatText = "What's better than disturbingly put together bow that shoots blood? A bigger bow of flesh that shoots organ chunks! Here, take your silly new weapon.";
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<SentientFleshBow>(), 1);
					SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
				}
				//flesh staff
				else if (Main.LocalPlayer.HasItem(ModContent.ItemType<OrroboroHeart>()) && Main.LocalPlayer.HasItem(ModContent.ItemType<FleshStaff>()))
				{
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OrroboroHeart>());
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<FleshStaff>());

					Main.npcChatText = "Eyes, eyes, and more eyes! I dunno how but it looks like your weird staff has grown. Hope it doesn't try to stalk you or something.";
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<SentientFleshStaff>(), 1);
					SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
				}
				//flesh whip
				else if (Main.LocalPlayer.HasItem(ModContent.ItemType<OrroboroHeart>()) && Main.LocalPlayer.HasItem(ModContent.ItemType<FleshWhip>()))
				{
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<OrroboroHeart>());
					Main.LocalPlayer.ConsumeItem(ModContent.ItemType<FleshWhip>());

					Main.npcChatText = "Whoa there, don't just start lashing peoples eyes out with that thing! Also not sure how it duplicated itself either. Oh well, have fun beating up random creatures with it!";
					Item.NewItem(NPC.GetSource_FromThis(), NPC.Center, ModContent.ItemType<SentientFleshWhip>(), 1);
					SoundEngine.PlaySound(SoundID.AbigailUpgrade, NPC.Center);
				}
				else
				{
					Main.npcChatText = "So, you want me to upgrade your weapons? Well, if you show me one of those peculiar flesh weapons and the heart of a giant beast, I can turn them into something a little crazy.";
				}
			}
		}

        public override string GetChat()
		{
			List<string> Dialogue = new List<string>
			{
                "This place is really funky. I love watching those weird flying things crash into the flesh pillars haha!",
				"Did you know there's a giant egg in some strange pit somewhere around here? That'd make enough scrambled eggs to last for weeks!",
				"You may call me an alchemist, I call it making dubious concoctions that I give to others for laughs.",
				"You may use my magic cauldron... Under the condition that you say 'The magic word'.",
				"My cauldron is pretty crazy. Last guy wanted to make a fire elixir to fight some giant bee, but instead he comically exploded.",
				"You know, the alchemy we do down here is not just some nerdy science. Nothing says alchemy more than throwing random things into a cauldron and saying gibberish afterward!",
				"If your alchemy creation doesn't completely blow up in your face, you're probably doing something wrong.",
			};

			return Main.rand.Next(Dialogue);
		}

		public override void AI()
		{
			Player player = Main.player[NPC.target];
			NPC.TargetClosest(true);

			NPC.spriteDirection = NPC.direction;

			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.homeless = false;
				NPC.homeTileX = -1;
				NPC.homeTileY = -1;
				NPC.netUpdate = true;
			}
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
    }
}