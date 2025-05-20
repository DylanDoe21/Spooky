/*
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using ReLogic.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Collections.Generic;

using Spooky.Core;
using Spooky.Content.Items.SpookyHell.EggEvent;
using Spooky.Content.Items.SpookyHell.Misc;
using Spooky.Content.NPCs.EggEvent.Projectiles;

namespace Spooky.Content.NPCs.EggEvent
{
    public class GiantBiomassPurple : ModNPC
    {
        public int SaveDirection;
        public float SaveRotation;

        Vector2 SavePosition;
        Vector2 SavePlayerPosition;

        private static Asset<Texture2D> NPCTexture;
        private static Asset<Texture2D> GlowTexture;

        public static readonly SoundStyle ScreamSound = new("Spooky/Content/Sounds/EggEvent/TongueBiterCharge", SoundType.Sound) { PitchVariance = 0.6f };

        public override void SetStaticDefaults()
        {
            NPCID.Sets.NPCBestiaryDrawOffset[NPC.type] = new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            //vector2
            writer.WriteVector2(SavePosition);
            writer.WriteVector2(SavePlayerPosition);

            //ints
            writer.Write(SaveDirection);

            //floats
            writer.Write(SaveRotation);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            //vector2
			SavePosition = reader.ReadVector2();
			SavePlayerPosition = reader.ReadVector2();

            //ints
            SaveDirection = reader.ReadInt32();

            //floats
            SaveRotation = reader.ReadSingle();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
        }

        public override void SetDefaults()
        {
            NPC.lifeMax = 500;
            NPC.damage = 50;
            NPC.defense = 10;
            NPC.width = 92;
            NPC.height = 86;
            NPC.npcSlots = 1f;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 0, 5, 0);
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit9;
			NPC.DeathSound = SoundID.NPCDeath12;
            NPC.aiStyle = -1;
        }

        public override void AI()
		{
        }
    }
}
*/