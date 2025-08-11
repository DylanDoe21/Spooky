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
    public class DownedRedowner : ModItem
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
			Flags.downedRotGourd = true;
            Flags.downedSpookySpirit = true;
            Flags.downedMoco = true;
            Flags.downedDaffodil = true;
            Flags.downedPandoraBox = true;
            Flags.downedEggEvent = true;
            Flags.downedOrroboro = true;
            Flags.downedBigBone = true;
            Flags.downedSpookFishron = true;

			return true;
		}
    }
}