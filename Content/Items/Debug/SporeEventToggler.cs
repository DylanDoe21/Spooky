using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Core;

namespace Spooky.Content.Items.Debug
{
    public class SporeEventToggler : ModItem
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
            if (Flags.SporeEventHappening)
            {
                Flags.SporeEventHappening = false;
                Flags.SporeEventTimeLeft = 0;
            }
            else
            {
                Flags.SporeEventHappening = true;
                Flags.SporeEventTimeLeft = 54000; //15 real-life minutes=
                Flags.SporeFogIntensity = 0.5f;
            }

			return true;
		}
    }
}