using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using System.Linq;

using Spooky.Core;

namespace Spooky.Content.Items
{
    public class DownedReset : ModItem
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
			Flags.downedRotGourd = false;
            Flags.downedSpookySpirit = false;
            Flags.downedMoco = false;
            Flags.downedDaffodil = false;
            Flags.downedPandoraBox = false;
            Flags.downedEggEvent = false;
            Flags.downedOrroboro = false;
            Flags.downedBigBone = false;
            Flags.downedSpookFishron = false;

			return true;
		}
    }
}