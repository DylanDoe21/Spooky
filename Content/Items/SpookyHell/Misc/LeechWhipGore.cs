using Terraria;
using Terraria.ModLoader;

namespace Spooky.Content.Items.SpookyHell.Misc
{
	public class LeechWhipGore : ModItem
	{
		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 22;
			Item.maxStack = 1;
		}

		public override bool ItemSpace(Player player)
		{
			return true;
		}

		public override bool OnPickup(Player player)
		{
            int LifeHealed = Main.rand.Next(5, 21);
			player.statLife += LifeHealed;
			player.HealEffect(LifeHealed, true);
			
			return false;
		}
	}
}