using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.UI;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Achievements;
using Spooky.Content.UserInterfaces;

namespace Spooky.Content.NPCs.Friendly
{
    public class DumbZomboid : ModNPC  
    {
        public Vector2 modifier = new(-200, -75);

        private static Asset<Texture2D> UITexture;

        public static readonly SoundStyle TalkSound = new("Spooky/Content/Sounds/LittleEye/Talk", SoundType.Sound) { Volume = 2f, PitchVariance = 0.75f };

        public override void Load()
		{
			UITexture = ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIDumbZomboid");
		}

        public override void SetStaticDefaults()
        {
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }
        
        public override void SetDefaults()
		{
            NPC.lifeMax = 250;
            NPC.defense = 5;
            NPC.width = 40;
			NPC.height = 44;
			NPC.friendly = true;
			NPC.immortal = true;
			NPC.dontTakeDamage = true;
			NPC.dontCountMe = true;
            NPC.npcSlots = 1f;
			NPC.knockBackResist = 0f;
            NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
            NPC.aiStyle = 0;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry) 
        {
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> 
            {
				new FlavorTextBestiaryInfoElement("Mods.Spooky.Bestiary.DumbZomboid"),
                new BestiaryBackgroundOverlay("Spooky/Content/Biomes/SpookyBiome_Background", Color.White)
			});
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

        public override void SetChatButtons(ref string button, ref string button2)
		{
			button = "";
		}

        public override string GetChat()
		{
            ModContent.GetInstance<MiscAchievementDumbZomboid>().DumbZomboidCondition.Complete();

            if (!Main.dedServ)
            {
                int DialogueChoice = Main.rand.Next(1, 3);

                DialogueChain chain = new();
                chain.Add(new(UITexture.Value, NPC,
                Language.GetTextValue("Mods.Spooky.Dialogue.DumbZomboidDialogue.Dialogue" + DialogueChoice),
                Language.GetTextValue("Mods.Spooky.Dialogue.DumbZomboidDialogue.DialoguePlayer" + DialogueChoice),
                TalkSound, 2f, 0f, modifier, NPCID: NPC.type))
                .Add(new(UITexture.Value, NPC, null, null, TalkSound, 2f, 0f, modifier, true));
                chain.OnPlayerResponseTrigger += PlayerResponse;
                chain.OnEndTrigger += EndDialogue;
                DialogueUI.Visible = true;
                DialogueUI.Add(chain);
            }

			return string.Empty;
		}

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;

            NPC.velocity.X = 0;

            if (Main.rand.NextBool(1500))
            {
                EmoteBubble.NewBubble(EmoteID.EmoteConfused, new WorldUIAnchor(NPC), 200);
            }
        }

        public static void PlayerResponse(Dialogue dialogue, string Text, int ID)
		{
			Dialogue newDialogue = new(ModContent.Request<Texture2D>("Spooky/Content/UserInterfaces/DialogueUIPlayer").Value, Main.LocalPlayer,
			Text, null, SoundID.Item1, 2f, 0f, default, NotPlayer: false);
			DialogueUI.Visible = true;
			DialogueUI.Add(newDialogue);
		}

		public static  void EndDialogue(Dialogue dialogue, int ID)
		{
			DialogueUI.Visible = false;
		}
    }
}