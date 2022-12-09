using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Spooky.Content.Events;

namespace Spooky.Content.Items.BossSummon
{
	public class EggEventStarter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Egg Event");
			Tooltip.SetDefault("~Test Item~");
		}

		public override void SetDefaults()
		{
			Item.width = 24;
			Item.height = 32;
			Item.consumable = true;
			Item.useTime = 45;
			Item.useAnimation = 45;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.maxStack = 20;
		}

		public override bool? UseItem(Player player)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && player.whoAmI == Main.myPlayer)
			{
				ModPacket packet = Mod.GetPacket();
                packet.Write((byte)SpookyMessageType.EggEventData);
                packet.Send();
			}
			else
			{
				EggEventWorld.EggEventActive = true;
			}
			return true;
		}
	}
}