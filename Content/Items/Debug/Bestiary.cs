using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Core;
using Spooky.Content.Tiles.Catacomb;
using Spooky.Content.Tiles.Catacomb.Ambient;
using Spooky.Content.Tiles.SpookyHell;

namespace Spooky.Content.Items.Debug
{
    public class Bestiary : ModItem
    {
        public override string Texture => "Spooky/Content/Items/BossSummon/CowBell";

        public override void SetDefaults()
        {                
            Item.width = 20;
            Item.height = 28;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(gold: 1);
        }

		public override bool? UseItem(Player player)
		{
			for (int i = 0; i < Main.npcFrameCount.Length; i++)
			{
				NPC npc = new NPC();
				npc.SetDefaults(i);

				for (int j = 0; j < 50; j++)
                {
					Main.BestiaryTracker.Kills.RegisterKill(npc);
                    Main.BestiaryTracker.Chats.SetWasChatWithDirectly(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[npc.type]);
                }
			}

			return true;
		}
    }
}