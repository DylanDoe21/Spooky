using Terraria;
using Terraria.ModLoader;

using Spooky.Core;

namespace Spooky.Content.Items.SpookyHell.Sentient
{
	[AutoloadEquip(EquipType.Head)]
	public class SentientCap : ModItem, ICauldronOutput
	{
		public override void SetDefaults()
		{
			Item.width = 28;
			Item.height = 20;
			Item.vanity = true;
			Item.rare = ModContent.RarityType<SentientRarity>();
		}

        public override void EquipFrameEffects(Player player, EquipType type)
        {
			player.GetModPlayer<SpookyPlayer>().SentientCap = true;
        }
    }
}